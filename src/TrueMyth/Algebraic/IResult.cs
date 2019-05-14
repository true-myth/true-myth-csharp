using System;

namespace TrueMyth.Algebraic
{
    public interface IResult<TVal,TErr> : IComparable, IComparable<IResult<TVal,TErr>>
    {
         IResult<T,TErr> And<T>(IResult<T,TErr> andResult);
         IResult<T,TErr> AndThen<T>(Func<TVal, IResult<T,TErr>> bindFn);
         IResult<T,TErr> Map<T>(Func<TVal,T> mapFn);
         T MapReturn<T>(Func<TVal,T> mapFn, T defaultValue);
         IResult<TVal,T> MapErr<T>(Func<TErr,T> mapFn);
         T Match<T>(Func<TVal,T> ok, Func<TErr,T> err);
         void Match(Action<TVal> ok, Action<TErr> err);
         IResult<TVal,T> Or<T>(IResult<TVal,T> defaultResult);
         IResult<TVal,T> OrElse<T>(Func<IResult<TVal,T>> elseFn);
         IResult<T,TErr> Select<T>(Func<TVal,T> mapFn);
         IResult<TVal,T> SelectErr<T>(Func<TErr,T> mapFn);
         IResult<T,TErr> SelectMany<T>(Func<TVal, IResult<T,TErr>> bindFn);
         IMaybe<TVal> ToMaybe();
         TVal UnwrapOr(TVal defaultValue);
         TVal UnwrapOrElse(Func<TErr,TVal> elseFn);
    }
}