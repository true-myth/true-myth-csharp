using System;
using System.Collections.Generic;

namespace TrueMyth.Algebraic
{
    using Unsafe;
    
    public sealed class Ok<TVal,TErr> : IResult<TVal,TErr>
    {
        private readonly TVal _value;

        internal Ok(TVal value) => _value = value;

        /// <exclude/>
        public IResult<T,TErr> And<T>(IResult<T,TErr> andResult) => andResult;
        /// <exclude/>
        public IResult<T,TErr> AndThen<T>(Func<TVal, IResult<T,TErr>> bindFn) => bindFn(this._value);
        /// <exclude/>
        public IResult<T,TErr> Map<T>(Func<TVal,T> mapFn) => new Ok<T,TErr>(mapFn(this._value));
        /// <exclude/>
        public T MapReturn<T>(Func<TVal,T> mapFn, T defaultValue) => mapFn(this._value);
        /// <exclude/>
        public IResult<TVal,T> MapErr<T>(Func<TErr,T> mapFn) => new Ok<TVal,T>(this._value);
        /// <exclude/>
        public T Match<T>(Func<TVal,T> ok, Func<TErr,T> err) => ok(this._value);
        /// <exclude/>
        public void Match(Action<TVal> ok, Action<TErr> err) => ok(this._value);
        /// <exclude/>
        public IResult<TVal,T> Or<T>(IResult<TVal,T> defaultResult) => new Ok<TVal,T>(this._value);
        /// <exclude/>
        public IResult<TVal,T> OrElse<T>(Func<IResult<TVal,T>> elseFn) => new Ok<TVal,T>(this._value);
        /// <exclude/>
        public IResult<T,TErr> Select<T>(Func<TVal,T> mapFn) => new Ok<T,TErr>(mapFn(this._value));
        /// <exclude/>
        public IResult<TVal,T> SelectErr<T>(Func<TErr,T> mapFn) => new Ok<TVal,T>(this._value);
        /// <exclude/>
        public IResult<T,TErr> SelectMany<T>(Func<TVal,IResult<T,TErr>> bindFn) => bindFn(this._value);
        /// <exclude/>
        public IMaybe<TVal> ToMaybe() => new Just<TVal>(this._value);
        /// <exclude/>
        public TVal UnwrapOr(TVal defaultValue) => this._value;
        /// <exclude/>
        public TVal UnwrapOrElse(Func<TErr,TVal> errFn) => this._value;

        #region Object Overrides
        /// <exclude/>
        public override string ToString() => $"Ok<{typeof(TVal)}>[{this._value}]";

        /// <exclude/>
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is IResult<TVal,TErr>))
            {
                return false;
            }

            switch (obj as IResult<TVal,TErr>)
            {
                case Ok<TVal,TErr> ok:
                    return EqualityComparer<TVal>.Default.Equals(this._value, ok.UnsafelyUnwrap());

                case Err<TVal,TErr> err:
                    return false;
            }

            throw new InvalidOperationException();
        }

        /// <exclude/>
        public override int GetHashCode()
        {
            unchecked
            {
                const int prime = 29;
                int hash = 19;
                hash = hash * prime + typeof(TVal).GetHashCode();
                hash = hash * prime + typeof(TErr).GetHashCode();
                hash = hash + prime + this._value.GetHashCode();
                return hash;
            }
        }
        #endregion
        
        #region IComparable Implementation
        /// <exclude/>
        public int CompareTo(IResult<TVal,TErr> otherResult)
        {
            if (otherResult == null)
            {
                return 1;
            }

            if (object.ReferenceEquals(this, otherResult))
            {
                return 0;
            }

            switch (otherResult)
            {
                case Ok<TVal,TErr> ok:
                    if (typeof(IComparable).IsAssignableFrom(typeof(TVal)))
                    {
                        var thisValue = this._value as IComparable;
                        var thatValue = otherResult.UnwrapOr(default(TVal));
                        return thisValue.CompareTo(thatValue);
                    }
                    else
                    {
                        return 0;
                    }
                case Err<TVal,TErr> err:
                    return 1;
            }

            throw new InvalidOperationException();
        }

        /// <exclude/>
        public int CompareTo(object obj)
        {
            if (!(obj is IResult<TVal,TErr>))
            {
                throw new ArgumentException($"Parameter of different type: {obj.GetType()}", nameof(obj));
            }

            return CompareTo(obj as IResult<TVal,TErr>);
        }
        #endregion
    }
}