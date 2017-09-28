using System;

namespace TrueMyth
{
    public enum MaybeVariant {
        Nothing,
        Some
    }
    
    public interface IMaybe<TValue>
    {
        MaybeVariant Variant { get; }
        IMaybe<TMapped> Map<TMapped>(Func<TValue, TMapped> mapFn);
        TValue Unwrap();
        TValue UnwrapOr(TValue defaultValue);
    }
}