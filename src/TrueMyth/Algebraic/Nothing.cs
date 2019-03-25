using System;

namespace TrueMyth.Algebraic
{
    public sealed class Nothing<T> : IMaybe<T>
    {
        internal Nothing() {}

        /// <exclude/>
        public IMaybe<U> And<U>(IMaybe<U> andMaybe) => new Nothing<U>();
        /// <exclude/>
        public IMaybe<U> AndThen<U>(Func<T,IMaybe<U>> bindFn) => new Nothing<U>();
        /// <exclude/>
        public IMaybe<U> Map<U>(Func<T,U> mapFn) => new Nothing<U>();
        /// <exclude/>
        public U MapReturn<U>(Func<T,U> mapFn, U defaultValue) => defaultValue;
        /// <exclude/>
        public U Match<U>(Func<T,U> just, Func<U> nothing) => nothing();
        /// <exclude/>
        public void Match(Action<T> just, Action nothing) => nothing();
        /// <exclude/>
        public IMaybe<T> Or(IMaybe<T> maybe) => maybe;
        /// <exclude/>
        public IMaybe<T> OrElse(Func<IMaybe<T>> elseFn) => elseFn();
        /// <exclude/>
        public IMaybe<U> Select<U>(Func<T,U> mapFn) => Map(mapFn);
        /// <exclude/>
        public IResult<T,E> ToResult<E>(E err) => new Err<T,E>(err);
        /// <exclude/>
        public IResult<T,E> ToResult<E>(Func<E> errFn) => new Err<T,E>(errFn());
        /// <exclude/>
        public T UnwrapOr(T defaultValue) => defaultValue;
        /// <exclude/>
        public T UnwrapOrElse(Func<T> elseFn) => elseFn();

        #region Object Overrides
        /// <exclude/>
        public override string ToString() => $"Nothing<{typeof(T)}>";

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
                    return false;

                case Nothing<T> nothing:
                    return true;
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
                    return -1;
                case Nothing<T> nothing:
                    return 0;
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