using System;

namespace TrueMyth
{
    public enum MaybeVariant
    {
        Nothing,
        Just
    }

    public interface IMaybe<TValue>
    {
        MaybeVariant Variant { get; }

        bool IsJust { get; }
        bool IsNothing { get; }
        IMaybe<TMapped> Map<TMapped>(Func<TValue, TMapped> selector);
        IMaybe<TMapped> Select<TMapped>(Func<TValue, TMapped> selector);
        TResult MapOr<TResult>(TResult orU, Func<TValue, TResult> selector);
        TResult SelectOr<TResult>(TResult orU, Func<TValue, TResult> selector);
        TResult MapOrElse<TResult>(Func<TResult> orElseFn, Func<TValue, TResult> selector);
        TResult SelectOrElse<TResult>(Func<TResult> orElseFn, Func<TValue, TResult> selector);
        TMatched Match<TMatched>(Matcher<TValue, TMatched> matcher);
        IMaybe<TValue> Or(IMaybe<TValue> orMaybe);
        IMaybe<TValue> OrElse(Func<IMaybe<TValue>> orElseFn);
        IMaybe<TResult> And<TResult>(IMaybe<TResult> mAnd);
        IMaybe<TResult> SelectMany<TResult>(Func<TValue, IMaybe<TResult>> selector);
        TValue UnsafelyUnwrap();
        TValue UnwrapOr(TValue defaultValue);
        IResult<TValue, TError> ToOkOrErr<TError>(TError error);
        IResult<TValue, TError> ToOkOrElseErr<TError>(Func<TError> elseFn);
        string ToString();
        bool Equals(IMaybe<TValue> comparison);
    }


    public static class Maybe
    {
        public static IMaybe<TValue> Of<TValue>(TValue value) =>
            value != null
                ? new Just<TValue>(value)
                : new Nothing<TValue>() as IMaybe<TValue>;

        private static TValue Unwrap<TValue>(IMaybe<TValue> maybeValue) => maybeValue.UnsafelyUnwrap();

        public static bool IsJust<TValue>(IMaybe<TValue> maybeValue) => maybeValue.Variant == MaybeVariant.Just;

        public static bool IsNothing<TValue>(IMaybe<TValue> maybeValue) => maybeValue.Variant == MaybeVariant.Nothing;

        public static IMaybe<TValue> Just<TValue>(TValue value) => new Just<TValue>(value);

        public static IMaybe<TValue> Nothing<TValue>() => new Nothing<TValue>();


        public static IMaybe<TMapped> Map<TValue, TMapped>(Func<TValue, TMapped> mapFunc, IMaybe<TValue> maybeValue) 
            => IsJust(maybeValue) ? Of(mapFunc(Unwrap(maybeValue))) : new Nothing<TMapped>();

        public static IMaybe<TMapped> Select<TValue, TMapped>(Func<TValue, TMapped> mapFunc, IMaybe<TValue> maybeValue) 
            => Map(mapFunc, maybeValue);

        public static TResult MapOr<TValue, TResult>(TResult orU, Func<TValue, TResult> mapFn, IMaybe<TValue> maybe)
            => IsJust(maybe) ? mapFn(Unwrap(maybe)) : orU;
        
        public static Func<IMaybe<TValue>, TResult> MapOr<TValue, TResult>(TResult orU, Func<TValue, TResult> mapFn)
            => m => MapOr(orU, mapFn, m);

        public static Func<Func<TValue, TResult>, Func<IMaybe<TValue>, TResult>> MapOr<TValue, TResult>(TResult orU)
            => fn => MapOr(orU, fn);

        public static TResult MapOrElse<TValue, TResult>(Func<TResult> orElseFn, Func<TValue, TResult> mapFn, IMaybe<TValue> maybe)
            => IsJust(maybe) ? mapFn(Unwrap(maybe)) : orElseFn();

        public static Func<IMaybe<TValue>, TResult> MapOrElse<TValue, TResult>(Func<TResult> orElseFn, Func<TValue, TResult> mapFn)
            => m => MapOrElse(orElseFn, mapFn, m);

        public static Func<Func<TValue, TResult>, Func<IMaybe<TValue>, TResult>> MapOrElse<TValue, TResult>(Func<TResult> orElseFn)
            => mapFn => MapOrElse(orElseFn, mapFn);

        public static IMaybe<TResult> SelectMany<TValue, TResult>(Func<TValue, IMaybe<TResult>> selector, IMaybe<TValue> maybe) 
            => IsJust(maybe) ? selector(Unwrap(maybe)) : Nothing<TResult>();
        

        public static TMatched Match<TValue, TMatched>(Matcher<TValue, TMatched> matcher, IMaybe<TValue> maybe)
            => maybe.Match(matcher);

        public static Func<IMaybe<TValue>, TMatched> Match<TValue, TMatched>(Matcher<TValue, TMatched> matcher)
            => maybe => Match(matcher, maybe);


        public static IMaybe<TResult> And<TValue, TResult>(IMaybe<TResult> andMaybe, IMaybe<TValue> maybe) =>
            IsJust(maybe) ? andMaybe : Nothing<TResult>();

        public static Func<IMaybe<TValue>, IMaybe<TResult>> And<TValue, TResult>(IMaybe<TResult> andMaybe)
            => maybe => And(andMaybe, maybe);

        public static IMaybe<U> AndThen<T, U>(Func<T, IMaybe<U>> thenFn, IMaybe<T> maybe)
            => IsJust(maybe) ? thenFn(Unwrap(maybe)) : Maybe.Nothing<U>();

        public static Func<IMaybe<T>, IMaybe<U>> AndThen<T, U>(Func<T, IMaybe<U>> thenFn)
            => maybe => AndThen(thenFn, maybe);

        public static IMaybe<U> Chain<T, U>(Func<T, IMaybe<U>> thenFn, IMaybe<T> maybe)
            => IsJust(maybe) ? thenFn(Unwrap(maybe)) : Maybe.Nothing<U>();

        public static Func<IMaybe<T>, IMaybe<U>> Chain<T, U>(Func<T, IMaybe<U>> thenFn)
            => maybe => AndThen(thenFn, maybe);

        public static IMaybe<U> FlatMap<T, U>(Func<T, IMaybe<U>> thenFn, IMaybe<T> maybe)
            => IsJust(maybe) ? thenFn(Unwrap(maybe)) : Maybe.Nothing<U>();

        public static Func<IMaybe<T>, IMaybe<U>> FlatMap<T, U>(Func<T, IMaybe<U>> thenFn)
            => maybe => AndThen(thenFn, maybe);


        public static IMaybe<TValue> Or<TValue>(IMaybe<TValue> mOr, IMaybe<TValue> maybe) 
            => IsJust(maybe) ? maybe : mOr;

        public static Func<IMaybe<TValue>, IMaybe<TValue>> Or<TValue>(IMaybe<TValue> mOr)
            => maybe => Or(mOr, maybe);

        public static IMaybe<TValue> OrElse<TValue>(Func<IMaybe<TValue>> elseFn, IMaybe<TValue> maybe) 
            => IsJust(maybe) ? maybe : elseFn();

        public static Func<IMaybe<TValue>, IMaybe<TValue>> OrElse<TValue>(Func<IMaybe<TValue>> elseFn)
            => maybe => OrElse(elseFn, maybe);


        public static TValue UnsafelyUnwrap<TValue>(IMaybe<TValue> maybeValue) 
            => maybeValue.UnsafelyUnwrap();

        public static TValue UnwrapOr<TValue>(TValue defaultValue, IMaybe<TValue> maybe) 
            => maybe.UnwrapOr(defaultValue);

        public static Func<IMaybe<TValue>, TValue> UnwrapOr<TValue>(TValue defaultValue)
            => maybe => UnwrapOr(defaultValue, maybe);

        public static TValue UnwrapOrElse<TValue>(Func<TValue> orElseFn, IMaybe<TValue> maybe) 
            => IsJust(maybe) ? Unwrap(maybe) : orElseFn();

        public static Func<IMaybe<TValue>, TValue> UnwrapOrElse<TValue>(Func<TValue> orElseFn)
            => maybe => UnwrapOrElse(orElseFn, maybe);


        public static IResult<TValue, TError> ToOkOrErr<TValue, TError>(TError error, IMaybe<TValue> maybe) 
            => IsJust(maybe)
                ? Result.Ok<TValue, TError>(Unwrap(maybe))
                : Result.Err<TValue, TError>(error) as IResult<TValue, TError>;

        public static Func<IMaybe<TValue>, IResult<TValue, TError>> ToOkOrErr<TValue, TError>(TError error)
            => maybe => ToOkOrErr(error, maybe);

        public static IResult<TValue, TError> ToOkOrElseErr<TValue, TError>(Func<TError> elseFn, IMaybe<TValue> maybe) 
            => IsJust(maybe)
                ? Result.Ok<TValue, TError>(Unwrap(maybe))
                : Result.Err<TValue, TError>(elseFn()) as IResult<TValue, TError>;

        public static Func<IMaybe<TValue>, IResult<TValue, TError>> ToOkOrElseErr<TValue, TError>(Func<TError> elseFn)
            => maybe => ToOkOrElseErr(elseFn, maybe);

        public static IMaybe<TValue> FromResult<TValue, TError>(IResult<TValue, TError> result) 
            => Result.IsOk(result) ? Just(Result.UnsafelyUnwrap(result)) : Nothing<TValue>();


        public static bool Equals<TValue>(IMaybe<TValue> maybe, IMaybe<TValue> comparison)
            => maybe.Equals(comparison);

        public static Func<IMaybe<TValue>, bool> Equals<TValue>(IMaybe<TValue> maybe)
            => comparison => Equals(maybe, comparison);


        public static IMaybe<U> Apply<TValue, U>(this IMaybe<Func<TValue, U>> maybeFn, IMaybe<TValue> maybe)
            => Match(
                new Matcher<TValue, IMaybe<U>>(
                    Just: val => maybeFn.Map(fn => fn(val)),
                    Nothing: () => Maybe.Nothing<U>()),
                maybe);

        public static Func<IMaybe<TValue>, IMaybe<U>> Apply<TValue, U>(IMaybe<Func<TValue, U>> maybeFn)
            => maybe => Apply(maybeFn, maybe);


        public static string ToString<TValue>(IMaybe<TValue> maybe)
            => maybe.ToString();
    }
}