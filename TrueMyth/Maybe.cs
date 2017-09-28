using System;

namespace TrueMyth
{
    public class Maybe
    {
        public static IMaybe<TValue> Of<TValue>(TValue value) => 
            value != null 
            ? new Some<TValue>(value) 
            : new Nothing<TValue>() as IMaybe<TValue>;

        public static IMaybe<TMapped> Map<TValue, TMapped>(Func<TValue, TMapped> mapFunc, IMaybe<TValue> maybeValue) =>
            IsSome(maybeValue) ? Of(mapFunc(Unwrap(maybeValue))) : new Nothing<TMapped>();

        public static bool IsSome<TValue>(IMaybe<TValue> maybeValue) => maybeValue.Variant == MaybeVariant.Some;
        public static bool IsNothing<TValue>(IMaybe<TValue> maybeValue) => maybeValue.Variant == MaybeVariant.Nothing;
        public static TValue Unwrap<TValue>(IMaybe<TValue> maybeValue) => maybeValue.Unwrap();
        public static TValue UnwrapOr<TValue>(TValue defaultValue, IMaybe<TValue> maybeValue) => 
            maybeValue.UnwrapOr(defaultValue);

        public static IMaybe<TValue> Nothing<TValue>() => new Nothing<TValue>();
    }
}