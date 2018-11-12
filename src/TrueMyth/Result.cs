using System;

namespace TrueMyth
{
    public sealed class Result<TValue, TError>
    {
        private readonly TValue _value;
        private readonly TError _error;
        private readonly bool _isOk;

        public bool IsOk => _isOk;
        public bool IsErr => !_isOk;

        private Result() {}

        private Result(TValue value, TError error, bool isOk)
        {
            _value = value;
            _error = error;
            _isOk = isOk;
        }

        public static Result<TValue, TError> Ok(TValue value)
        {
            return new Result<TValue, TError>(value, default(TError), true);
        }
	
        public static Result<TValue, TError> Err(TError err)
        {
            return new Result<TValue, TError>(default(TValue), err, false);
        }

        public TValue UnsafelyUnwrap() => _isOk ? _value : throw new InvalidOperationException("Invalid request to unwrap value.");
        public TError UnsafelyUnwrapErr() => !_isOk ? _error : throw new InvalidOperationException("Invalid request to unwrap error.");

        public static implicit operator TValue(Result<TValue, TError> result) => 
            result._isOk ? result._value : throw new InvalidOperationException("Invalid conversion to value type.");

        public static implicit operator TError(Result<TValue, TError> result) =>
            !result._isOk ? result._error : throw new InvalidOperationException("Invalid conversion to error type.");

        public static implicit operator Result<TValue,TError>(TValue value) => new Result<TValue,TError>(value, default(TError), true);
        public static implicit operator Result<TValue,TError>(TError error) => new Result<TValue,TError>(default(TValue), error, false);
    }
}