using System;

namespace TrueMyth.Algebraic
{
    public static class Result
    {
        public static IResult<TVal,TErr> From<TVal, TErr>(IMaybe<TVal> maybe, TErr error) =>
            maybe.Match(val => new Ok<TVal,TErr>(val) as IResult<TVal,TErr>,
                        () => new Err<TVal,TErr>(error) as IResult<TVal,TErr>);

        public static IResult<TVal,TErr> Ok<TVal,TErr>(TVal value) => new Ok<TVal,TErr>(value);
        public static IResult<TVal,TErr> Err<TVal,TErr>(TErr error) => new Err<TVal,TErr>(error);

        public static IResult<TVal,TErr> Try<TVal,TErr>(Func<TVal> fn, TErr error)
        {
            try
            {
                return new Ok<TVal,TErr>(fn());
            }
            catch
            {
                return new Err<TVal,TErr>(error);
            }
        }

        public static IResult<TVal,TErr> Try<TVal,TErr>(Func<TVal> fn, Func<TErr> errFn)
        {
            try
            {
                return new Ok<TVal,TErr>(fn());
            }
            catch
            {
                return new Err<TVal,TErr>(errFn());
            }
        }
    }
}