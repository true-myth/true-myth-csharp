using System;

namespace TrueMyth
{
    /// <summary>
    /// Create an instance of `Maybe.Just` with `new`.
    /// </summary>
    /// <typeparam name="TValue">The type of the wrapped item.</typeparam>
    public sealed class Just<TValue> : IMaybe<TValue>, IEquatable<IMaybe<TValue>>
    {
        private readonly TValue _value;

        public MaybeVariant Variant => MaybeVariant.Just;

        public Just(TValue value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value), "Tried to construct `Just` with `null`");

            _value = value;
        }

        public override string ToString()
            => $"{Variant.ToString()}({_value.ToString()})";

        public bool IsJust() => true;

        public bool IsNothing() => false;
        public IMaybe<TMapped> Map<TMapped>(Func<TValue, TMapped> selector)
            => new Just<TMapped>(selector(_value));

        public IMaybe<TMapped> Select<TMapped>(Func<TValue, TMapped> selector) 
            => new Just<TMapped>(selector(_value));

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

        public IResult<TValue, TError> ToOkOrErr<TError>(TError error)
            => Maybe.ToOkOrErr(error, this);

        public IResult<TValue, TError> ToOkOrElseErr<TError>(Func<TError> elseFn)
            => Maybe.ToOkOrElseErr(elseFn, this);

        public TValue UnsafelyUnwrap() => _value;
        public TValue UnwrapOr(TValue defaultValue) => _value;

        public TMatched Match<TMatched>(Matcher<TValue, TMatched> matcher)
            => matcher.Just(_value);

        public bool Equals(IMaybe<TValue> comparison)
            => Maybe.IsJust(comparison) && _value.Equals(comparison.UnsafelyUnwrap());
    }
}
