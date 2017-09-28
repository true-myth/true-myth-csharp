using System;

namespace TrueMyth
{
    public sealed class Some<TValue> : IMaybe<TValue>
    {
        private readonly TValue _value;
        
        public MaybeVariant Variant => MaybeVariant.Some;

        public Some(TValue value)
        {
            if (value == null)
                throw new Exception("Tried to construct `Some` with `null`");
            
            _value = value;
        }

        public IMaybe<TMapped> Map<TMapped>(Func<TValue, TMapped> mapFn) => new Some<TMapped>(mapFn(_value));
        public TValue Unwrap() => _value;
        public TValue UnwrapOr(TValue defaultValue) => _value;
    }
}