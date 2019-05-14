using System;

namespace TrueMyth.Algebraic
{
    public interface IMaybe<TVal> : IComparable, IComparable<IMaybe<TVal>>
    {
        IMaybe<T> And<T>(IMaybe<T> andMaybe);
        IMaybe<T> AndThen<T>(Func<TVal,IMaybe<T>> bindFn);
        IMaybe<T> Map<T>(Func<TVal,T> mapFn);
        T MapReturn<T>(Func<TVal,T> mapFn, T defaultValue);
        T Match<T>(Func<TVal,T> just, Func<T> nothing);
        void Match(Action<TVal> just, Action nothing);
        IMaybe<TVal> Or(IMaybe<TVal> maybe);
        IMaybe<TVal> OrElse(Func<IMaybe<TVal>> elseFn);
        IMaybe<T> Select<T>(Func<TVal,T> mapFn);
        IResult<TVal,TErr> ToResult<TErr>(TErr err);
        IResult<TVal,TErr> ToResult<TErr>(Func<TErr> errFn);
        TVal UnwrapOr(TVal defaultValue);
        TVal UnwrapOrElse(Func<TVal> elseFn);
    }
}
