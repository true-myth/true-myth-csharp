using System;

namespace TrueMyth
{
    public sealed class Nothing<TValue> : IMaybe<TValue>
    {
        public MaybeVariant Variant => MaybeVariant.Nothing;

        public IMaybe<TMapped> Map<TMapped>(Func<TValue, TMapped> mapFn) => new Nothing<TMapped>();
        public TValue Unwrap() => throw new Exception("Tried to `unwrap(Nothing)`");
        public TValue UnwrapOr(TValue defaultValue) => defaultValue;
    }
}