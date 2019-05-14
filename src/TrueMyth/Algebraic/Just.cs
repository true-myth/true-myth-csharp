using System;
using System.Collections.Generic;

namespace TrueMyth.Algebraic
{
    public sealed class Just<T> : IMaybe<T>
    {
        private readonly T _value;

        internal Just(T value) => _value = value;

        /// <exclude/>
        public IMaybe<U> And<U>(IMaybe<U> andMaybe) => andMaybe;
        /// <exclude/>
        public IMaybe<U> AndThen<U>(Func<T,IMaybe<U>> bindFn) => bindFn(this._value);
        /// <exclude/>
        public IMaybe<U> Map<U>(Func<T,U> mapFn) => new Just<U>(mapFn(this._value));
        /// <exclude/>
        public U MapReturn<U>(Func<T,U> mapFn, U defaultValue) => mapFn(this._value);
        /// <exclude/>
        public U Match<U>(Func<T,U> just, Func<U> nothing) => just(this._value);
        /// <exclude/>
        public void Match(Action<T> just, Action nothing) => just(this._value);
        /// <exclude/>
        public IMaybe<T> Or(IMaybe<T> maybe) => this;
        /// <exclude/>
        public IMaybe<T> OrElse(Func<IMaybe<T>> elseFn) => this;
        /// <exclude/>
        public IMaybe<U> Select<U>(Func<T,U> mapFn) => Map(mapFn);
        /// <exclude/>
        public IResult<T,E> ToResult<E>(E err) => new Ok<T,E>(this._value);
        /// <exclude/>
        public IResult<T,E> ToResult<E>(Func<E> errFn) => new Ok<T,E>(this._value);
        /// <exclude/>
        public T UnwrapOr(T defaultValue) => this._value;
        /// <exclude/>
        public T UnwrapOrElse(Func<T> elseFn) => this._value;

        #region Object Overrides
        /// <exclude/>
        public override string ToString() => $"Just<{typeof(T)}>[{this._value}]";

        /// <exclude/>
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is IMaybe<T>))
            {
                return false;
            }

            switch (obj as IMaybe<T>)
            {
                case Just<T> just:
                    return EqualityComparer<T>.Default.Equals(this._value, just.UnwrapOr(default(T)));

                case Nothing<T> nothing:
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
                hash = hash * prime + typeof(T).GetHashCode();
                hash = hash * prime + this._value.GetHashCode();
                return hash;
            }
        }
        #endregion

        #region IComparable Implementation
        /// <exclude/>
        public int CompareTo(IMaybe<T> otherMaybe)
        {
            if (otherMaybe == null)
            {
                return 1;
            }

            if (object.ReferenceEquals(this, otherMaybe))
            {
                return 0;
            }

            switch (otherMaybe)
            {
                case Just<T> just:
                    if (typeof(IComparable).IsAssignableFrom(typeof(T)))
                    {
                        var justThis = this._value as IComparable;
                        var justThat = otherMaybe.UnwrapOr(default(T));
                        return justThis.CompareTo(justThat);
                    }
                    else
                    {
                        return 0;
                    }
                case Nothing<T> nothing:
                    return 1;
            }

            throw new InvalidOperationException();
        }

        /// <exclude/>
        public int CompareTo(object obj)
        {
            if (!(obj is IMaybe<T>))
            {
                throw new ArgumentException($"Parameter of different type: {obj.GetType()}", nameof(obj));
            }

            return CompareTo(obj as IMaybe<T>);
        }
        #endregion
    }
}