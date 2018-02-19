using System;

namespace TrueMyth
{
    public sealed class Nothing<TValue> : IMaybe<TValue>, IEquatable<IMaybe<TValue>>
    {
        public MaybeVariant Variant => MaybeVariant.Nothing;

        public bool IsJust => false;
        public bool IsNothing => true;

        public override string ToString()
            => $"{Variant.ToString()}";

        public IMaybe<TMapped> Map<TMapped>(Func<TValue, TMapped> selector)
            => new Nothing<TMapped>();

        public IMaybe<TMapped> Select<TMapped>(Func<TValue, TMapped> selector) 
            => new Nothing<TMapped>();

        public TResult SelectOr<TResult>(TResult orU, Func<TValue, TResult> selector) =>
            Maybe.MapOr(orU, selector, this);

        public TResult SelectOrElse<TResult>(Func<TResult> orElseFn, Func<TValue, TResult> selector) =>
            Maybe.MapOrElse(orElseFn, selector, this);

        public IMaybe<TValue> Or(IMaybe<TValue> orMaybe) => Maybe.Or(orMaybe, this);

        public IMaybe<TValue> OrElse(Func<IMaybe<TValue>> orElseFn) => Maybe.OrElse(orElseFn, this);

        public IMaybe<TResult> And<TResult>(IMaybe<TResult> mAnd) 
            => Maybe.And(mAnd, this);

        public IMaybe<TResult> SelectMany<TResult>(Func<TValue, IMaybe<TResult>> selector) =>
            Maybe.SelectMany(selector, this);

        public TValue UnsafelyUnwrap() 
            => throw new Exception("Tried to `Unwrap(Nothing)`");
        public TValue UnwrapOr(TValue defaultValue) 
            => defaultValue;

        public IResult<TValue, TError> ToOkOrErr<TError>(TError error)
            => Maybe.ToOkOrErr(error, this);

        public IResult<TValue, TError> ToOkOrElseErr<TError>(Func<TError> elseFn)
            => Maybe.ToOkOrElseErr(elseFn, this);

        public TMatched Match<TMatched>(Matcher<TValue, TMatched> matcher)
            => matcher.Nothing();

        public bool Equals(IMaybe<TValue> comparison)
            => Maybe.IsNothing(comparison);

        //public IMaybe<U> Apply<U>(IMaybe<Func<TValue, U>> maybeFn)
        //    => Maybe.Nothing<U>();
    }
}
