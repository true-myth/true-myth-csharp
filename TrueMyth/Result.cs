using System;

namespace TrueMyth
{
    public interface IResult<TValue, TError>
    {
        TValue UnsafelyUnwrap();
        TError UnsafelyUnwrapErr();
        IResult<TMapped, TError> Select<TMapped>(Func<TValue, TMapped> selector);
        IResult<TSelected, TError> SelectMany<TSelected>(Func<TValue, IResult<TSelected, TError>> selector);
        bool IsOk();
        bool IsErr();
    }

    public static class Result
    {
        public static Ok<TValue, TError> Ok<TValue, TError>(TValue value) => new Ok<TValue, TError>(value);
        public static Err<TValue, TError> Err<TValue, TError>(TError error) => new Err<TValue, TError>(error);
        public static bool IsOk<TValue, TError>(IResult<TValue, TError> result) => result.IsOk();
        public static bool IsErr<TValue, TError>(IResult<TValue, TError> result) => result.IsErr();

        public static IResult<TMapped, TError> Select<TValue, TMapped, TError>(
            Func<TValue, TMapped> selector,
            IResult<TValue, TError> result
        ) => IsOk(result)
            ? new Ok<TMapped, TError>(selector(UnsafelyUnwrap(result)))
            : result as IResult<TMapped, TError>;

        public static IResult<TSelected, TError> SelectMany<TValue, TSelected, TError>(
            Func<TValue, IResult<TSelected, TError>> selector,
            IResult<TValue, TError> result
        ) => IsOk(result)
            ? selector(UnsafelyUnwrap(result))
            : result as IResult<TSelected, TError>;

        public static TValue UnsafelyUnwrap<TValue, TError>(IResult<TValue, TError> result) => result.UnsafelyUnwrap();
    }

    public class Ok<TValue, TError> : IResult<TValue, TError>
    {
        private readonly TValue _value;

        public Ok(TValue value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value), "Tried to construct `Ok` with `null`");

            _value = value;
        }

        public bool IsOk() => true;
        public bool IsErr() => false;

        public IResult<TMapped, TError> Select<TMapped>(Func<TValue, TMapped> selector) =>
            Result.Select(selector, this);
        
        public IResult<TSelected, TError> SelectMany<TSelected>(Func<TValue, IResult<TSelected, TError>> selector) =>
            Result.SelectMany(selector, this);

        public TValue UnsafelyUnwrap() => _value;

        public TError UnsafelyUnwrapErr()
        {
            throw new Exception("Tried to `UnsafelyUnwrapError with Ok`");
        }
    }

    public class Err<TValue, TError> : IResult<TValue, TError>
    {
        private readonly TError _error;

        public Err(TError error)
        {
            if (error == null)
                throw new ArgumentNullException(nameof(error), "Tried to construct `Err` with `null`");

            _error = error;
        }

        public bool IsOk() => false;
        public bool IsErr() => true;

        public IResult<TMapped, TError> Select<TMapped>(Func<TValue, TMapped> selector) =>
            Result.Select(selector, this);

        public IResult<TSelected, TError> SelectMany<TSelected>(Func<TValue, IResult<TSelected, TError>> selector) =>
            Result.SelectMany(selector, this);

        public TValue UnsafelyUnwrap()
        {
            throw new Exception("Tried to `UnsafelyUnwrapErr` with an `Error``");
        }

        public TError UnsafelyUnwrapErr() => _error;
    }
}