using System;

namespace TrueMyth.Unsafe
{
    using TrueMyth;
    using TrueMyth.Algebraic;

    /// <summary>
    /// Extensions to the <see cref="Maybe{T}"/> and <see cref="Result{T,Err}"/> implementations; the use of these
    /// methods is discouraged by organizing them this way.
    /// </summary>
    public static class UnsafeExtensions
    {
        /// <summary>
        /// Get the <c>TValue</c> value out of the <c>Maybe&lt;TValue&gt;</c>. Returns the content of a <b>Just</b>, but <em>throws if the <c>Maybe</c> is <b>Nothing</b></em>.
        /// Prefer to use <see cref="Maybe{T}.UnwrapOr(T)"/> or <see cref="Maybe{T}.UnwrapOrElse(Func{T})"/>.
        /// </summary>
        public static T UnsafelyUnwrap<T>(this Maybe<T> maybe) => 
            maybe.UnwrapOrElse(() => throw new InvalidOperationException($"Invalid attempt to unwrap {maybe}."));

        public static T UnsafelyUnwrap<T>(this IMaybe<T> maybe) => 
            maybe.UnwrapOrElse(() => throw new InvalidOperationException($"Invalid attempt to unwrap {maybe}."));

        /// <summary>
        /// Get the value out of the <c>Result</c>. Returns the content of an <b>Ok</b> but <em>throws if the result is <b>Err</b></em>.
        /// Prefer to use <see cref="Result{T,E}.UnwrapOr(T)"/> or <see cref="Result{T,E}.UnwrapOrElse(Func{E,T})"/>
        /// </summary>
        public static T UnsafelyUnwrap<T,E>(this Result<T,E> result) => 
            result.UnwrapOrElse(err => throw new InvalidOperationException($"Invalid request to unwrap {result} value."));

        public static T UnsafelyUnwrap<T,E>(this IResult<T,E> result) =>
            result.UnwrapOrElse(err => throw new InvalidOperationException($"Invalid request to unwrap {result} value."));

        /// <summary>
        /// Get the error out of the <c>Result</c>. Returns the content of an <b>Err</b>, but <em>throws if the result is <b>Ok</b></em>.
        /// Prefer to use <see cref="Result{T,E}.UnwrapOrElse(Func{E,T})"/>.
        /// </summary>
        public static E UnsafelyUnwrapErr<T,E>(this Result<T,E> result) => 
            result.Match(t => throw new InvalidOperationException($"Invalid request to unwrap {result} error."), e => e);

        public static E UnsafelyUnwrapErr<T,E>(this IResult<T,E> result) =>
            result.Match(t => throw new InvalidOperationException($"Invalid request to unwrap {result} error."), e => e);
    }
}