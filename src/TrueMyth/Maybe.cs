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

        bool IsJust();
        bool IsNothing();
        IMaybe<TMapped> Select<TMapped>(Func<TValue, TMapped> selector);
        TResult SelectOr<TResult>(TResult orU, Func<TValue, TResult> selector);
        TResult SelectOrElse<TResult>(Func<TResult> orElseFn, Func<TValue, TResult> selector);
        IMaybe<TValue> Or(IMaybe<TValue> orMaybe);
        IMaybe<TValue> OrElse(Func<IMaybe<TValue>> orElseFn);
        IMaybe<TResult> And<TResult>(IMaybe<TResult> mAnd);
        IMaybe<TResult> SelectMany<TResult>(Func<TValue, IMaybe<TResult>> selector);
        TValue UnsafelyUnwrap();
        TValue UnwrapOr(TValue defaultValue);
        IResult<TValue, TError> ToOkOrErr<TError>(TError error);
        IResult<TValue, TError> ToOkOrElseErr<TError>(Func<TError> elseFn);
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

        public static IMaybe<TMapped> Select<TValue, TMapped>(
            IMaybe<TValue> maybeValue,
            Func<TValue, TMapped> mapFunc
        ) => IsJust(maybeValue) ? Of(mapFunc(Unwrap(maybeValue))) : new Nothing<TMapped>();

        public static TResult MapOr<TValue, TResult>(TResult orU, Func<TValue, TResult> mapFn, IMaybe<TValue> maybe) =>
            IsJust(maybe) ? mapFn(Unwrap(maybe)) : orU;

        public static TResult MapOrElse<TValue, TResult>(
            Func<TResult> orElseFn,
            Func<TValue, TResult> mapFn,
            IMaybe<TValue> maybe
        ) => IsJust(maybe) ? mapFn(Unwrap(maybe)) : orElseFn();

        public static IMaybe<TResult> And<TValue, TResult>(IMaybe<TResult> andMaybe, IMaybe<TValue> maybe) =>
            IsJust(maybe) ? andMaybe : Nothing<TResult>();

        public static IMaybe<TResult> SelectMany<TValue, TResult>(
            Func<TValue, IMaybe<TResult>> selector,
            IMaybe<TValue> maybe
        ) => IsJust(maybe) ? selector(Unwrap(maybe)) : Nothing<TResult>();

        public static IMaybe<TValue> Or<TValue>(IMaybe<TValue> mOr, IMaybe<TValue> maybe) =>
            IsJust(maybe) ? maybe : mOr;

        public static IMaybe<TValue> OrElse<TValue>(Func<IMaybe<TValue>> elseFn, IMaybe<TValue> maybe) =>
            IsJust(maybe) ? maybe : elseFn();

        public static TValue UnsafelyUnwrap<TValue>(IMaybe<TValue> maybeValue) => maybeValue.UnsafelyUnwrap();

        public static TValue UnwrapOr<TValue>(TValue defaultValue, IMaybe<TValue> maybeValue) =>
            maybeValue.UnwrapOr(defaultValue);

        public static TValue UnwrapOrElse<TValue>(Func<TValue> orElseFn, IMaybe<TValue> maybe) =>
            IsJust(maybe) ? Unwrap(maybe) : orElseFn();

        public static IResult<TValue, TError> ToOkOrErr<TValue, TError>(TError error, IMaybe<TValue> maybe) =>
            IsJust(maybe)
                ? Result.Ok<TValue, TError>(Unwrap(maybe))
                : Result.Err<TValue, TError>(error) as IResult<TValue, TError>;

        public static IResult<TValue, TError>
            ToOkOrElseErr<TValue, TError>(Func<TError> elseFn, IMaybe<TValue> maybe) =>
            IsJust(maybe)
                ? Result.Ok<TValue, TError>(Unwrap(maybe))
                : Result.Err<TValue, TError>(elseFn()) as IResult<TValue, TError>;

        public static IMaybe<TValue> FromResult<TValue, TError>(IResult<TValue, TError> result) =>
            Result.IsOk(result) ? Just(Result.UnsafelyUnwrap(result)) : Nothing<TValue>();
    }

    /// <summary>
    /// Create an instance of `Maybe.Just` with `new`.
    /// </summary>
    /// <typeparam name="TValue">The type of the wrapped item.</typeparam>
    public sealed class Just<TValue> : IMaybe<TValue>
    {
        private readonly TValue _value;

        public MaybeVariant Variant => MaybeVariant.Just;

        public Just(TValue value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value), "Tried to construct `Just` with `null`");

            _value = value;
        }

        public bool IsJust() => true;

        public bool IsNothing() => false;

        public IMaybe<TMapped> Select<TMapped>(Func<TValue, TMapped> selector) => new Just<TMapped>(selector(_value));

        public TResult SelectOr<TResult>(TResult orU, Func<TValue, TResult> selector) =>
            Maybe.MapOr(orU, selector, this);

        public TResult SelectOrElse<TResult>(Func<TResult> orElseFn, Func<TValue, TResult> selector) =>
            Maybe.MapOrElse(orElseFn, selector, this);

        public IMaybe<TValue> Or(IMaybe<TValue> orMaybe) => Maybe.Or(orMaybe, this);

        public IMaybe<TValue> OrElse(Func<IMaybe<TValue>> orElseFn) => Maybe.OrElse(orElseFn, this);

        public IMaybe<TResult> And<TResult>(IMaybe<TResult> mAnd)
        {
            throw new NotImplementedException();
        }

        public IMaybe<TResult> SelectMany<TResult>(Func<TValue, IMaybe<TResult>> selector) =>
            Maybe.SelectMany(selector, this);

        public IResult<TValue, TError> ToOkOrErr<TError>(TError error)
        {
            throw new NotImplementedException();
        }

        public IResult<TValue, TError> ToOkOrElseErr<TError>(Func<TError> elseFn)
        {
            throw new NotImplementedException();
        }

        public TValue UnsafelyUnwrap() => _value;
        public TValue UnwrapOr(TValue defaultValue) => _value;
    }

    public sealed class Nothing<TValue> : IMaybe<TValue>
    {
        public MaybeVariant Variant => MaybeVariant.Nothing;

        public bool IsJust() => false;

        public bool IsNothing() => true;

        public IMaybe<TMapped> Select<TMapped>(Func<TValue, TMapped> selector) => new Nothing<TMapped>();

        public TResult SelectOr<TResult>(TResult orU, Func<TValue, TResult> selector) =>
            Maybe.MapOr(orU, selector, this);

        public TResult SelectOrElse<TResult>(Func<TResult> orElseFn, Func<TValue, TResult> selector) =>
            Maybe.MapOrElse(orElseFn, selector, this);

        public IMaybe<TValue> Or(IMaybe<TValue> orMaybe) => Maybe.Or(orMaybe, this);

        public IMaybe<TValue> OrElse(Func<IMaybe<TValue>> orElseFn) => Maybe.OrElse(orElseFn, this);

        public IMaybe<TResult> And<TResult>(IMaybe<TResult> mAnd) => Maybe.And(mAnd, this);

        public IMaybe<TResult> SelectMany<TResult>(Func<TValue, IMaybe<TResult>> selector) =>
            Maybe.SelectMany(selector, this);

        public TValue UnsafelyUnwrap() => throw new Exception("Tried to `unwrap(Nothing)`");
        public TValue UnwrapOr(TValue defaultValue) => defaultValue;

        public IResult<TValue, TError> ToOkOrErr<TError>(TError error)
        {
            throw new NotImplementedException();
        }

        public IResult<TValue, TError> ToOkOrElseErr<TError>(Func<TError> elseFn)
        {
            throw new NotImplementedException();
        }
    }
}