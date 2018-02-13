using System;

namespace TrueMyth
{
    /** A lightweight object defining how to handle each variant of a Maybe. */
    public class Matcher<T>
    {
        IMaybe<T> _maybe;

        public Matcher()
        {
            _maybe = Maybe.Nothing<T>();
        }

        internal Matcher(IMaybe<T> maybe) : this()
            => _maybe = maybe;
    }

    public class Matcher<TIn, TOut>
    {
        public Func<TIn, TOut> Just { get; }
        public Func<TOut> Nothing { get; }

        public Matcher(Func<TIn, TOut> Just, Func<TOut> Nothing)
        {
            this.Just = Just;
            this.Nothing = Nothing;
        }
    }
}
