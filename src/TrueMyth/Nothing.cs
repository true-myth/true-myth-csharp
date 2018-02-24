using System;
using System.Collections.Generic;

namespace TrueMyth
{
    public sealed class Nothing<TValue> : IMaybe<TValue>, IEquatable<IMaybe<TValue>>, IEquatable<IMaybeVariant>
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

        public TResult MapOr<TResult>(TResult orU, Func<TValue, TResult> selector) 
            => orU;

        public TResult SelectOr<TResult>(TResult orU, Func<TValue, TResult> selector)
            => orU;

        public TResult MapOrElse<TResult>(Func<TResult> orElseFn, Func<TValue, TResult> selector)
            => orElseFn();

        public TResult SelectOrElse<TResult>(Func<TResult> orElseFn, Func<TValue, TResult> selector)
            => orElseFn();

        public IMaybe<TValue> Or(IMaybe<TValue> orMaybe) 
            => orMaybe;

        public IMaybe<TValue> OrElse(Func<IMaybe<TValue>> orElseFn) 
            => orElseFn();

        public IMaybe<TResult> And<TResult>(IMaybe<TResult> mAnd) 
            => Maybe.Nothing<TResult>();

        public IMaybe<TResult> AndThen<TResult>(Func<TValue, IMaybe<TResult>> thenFn)
            => Maybe.Nothing<TResult>();

        public IMaybe<TResult> SelectMany<TResult>(Func<TValue, IMaybe<TResult>> selector) 
            => Maybe.SelectMany(selector, this);

        public TValue UnsafelyUnwrap() 
            => throw new Exception("Tried to `Unwrap(Nothing)`");

        public TValue UnwrapOr(TValue defaultValue) 
            => defaultValue;

        public TValue UnwrapOrElse(Func<TValue> orElseFn) 
            => orElseFn();

        public IResult<TValue, TError> ToOkOrErr<TError>(TError error)
            => Maybe.ToOkOrErr(error, this);

        public IResult<TValue, TError> ToOkOrElseErr<TError>(Func<TError> elseFn)
            => Maybe.ToOkOrElseErr(elseFn, this);

        public TMatched Match<TMatched>(Matcher<TValue, TMatched> matcher)
            => matcher.Nothing();

        public bool Equals(IMaybe<TValue> comparison)
            => Maybe.IsNothing(comparison);

        // Ensure a Nothing<string> and a Nothing<int> are both still considered a Nothing
        public bool Equals(IMaybeVariant other)
            => Variant == other.Variant;

        public override bool Equals(object obj)
            => obj is IMaybeVariant mv && Variant == mv.Variant;

        public override int GetHashCode()
            => 0;
    }
}
