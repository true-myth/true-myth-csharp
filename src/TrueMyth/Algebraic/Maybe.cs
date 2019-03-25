using System;

namespace TrueMyth.Algebraic
{
    public static class Maybe
    {
        public static IMaybe<TVal> From<TVal>(Nullable<TVal> nullable) where TVal : struct => 
            nullable.HasValue
            ? Maybe.Of(nullable.Value)
            : new Nothing<TVal>() as IMaybe<TVal>;

        public static IMaybe<TVal> From<TVal,TErr>(IResult<TVal,TErr> result) => 
            result.Match(
                val => new Just<TVal>(val) as IMaybe<TVal>,
                err => new Nothing<TVal>());

        public static IMaybe<TVal> Of<TVal>(TVal value) => 
            value == null
            ? new Nothing<TVal>() as IMaybe<TVal> 
            : new Just<TVal>(value) as IMaybe<TVal>;

        public static IMaybe<TVal> Nothing<TVal>() => new Nothing<TVal>();
    }
}