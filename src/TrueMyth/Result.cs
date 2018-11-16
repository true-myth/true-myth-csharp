using System;
using System.Collections.Generic;

namespace TrueMyth
{
    /// <summary>
    /// Something
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TError"></typeparam>
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

        public static Result<TValue, TError> Ok(TValue value) => new Result<TValue, TError>(value, default(TError), true);
        public static Result<TValue, TError> Err(TError err) => new Result<TValue, TError>(default(TValue), err, false);

        public TValue UnsafelyUnwrap() => _isOk ? _value : throw new InvalidOperationException("Invalid request to unwrap value.");
        public TError UnsafelyUnwrapErr() => !_isOk ? _error : throw new InvalidOperationException("Invalid request to unwrap error.");

        public TValue UnwrapOr(TValue defaultValue) => this._isOk ? _value : defaultValue;

        public TValue UnwrapOrElse(Func<TError,TValue> elseFn) => this._isOk ? _value : elseFn(this._error);

        public Result<UValue, TError> Select<UValue>(Func<TValue,UValue> mapFn) => this._isOk
            ? new Result<UValue,TError>(mapFn(this._value), default(TError), true)
            : new Result<UValue,TError>(default(UValue), this._error, false);

        public Result<TValue, UError> SelectErr<UError>(Func<TError,UError> mapFn) => !this._isOk
            ? new Result<TValue,UError>(default(TValue), mapFn(this._error), false)
            : new Result<TValue,UError>(this._value, default(UError), true);

        public UValue SelectOrDefault<UValue>(Func<TValue,UValue> mapFn, UValue defaultValue) => this._isOk ? mapFn(this._value) : defaultValue;
        public UValue SelectOrElse<UValue>(Func<TValue,UValue> mapFn, Func<TError,UValue> mapErrFn) => this._isOk ? mapFn(this._value) : mapErrFn(this._error);

        public T Match<T>(Func<TValue,T> ok, Func<TError,T> err) => this._isOk ? ok(this._value) : err(this._error);

        public Result<TValue, TError> Or(Result<TValue,TError> r1) => this._isOk ? this : r1;

        public TValue OrElse(Func<Result<TValue,TError>> elseFn) => this._isOk ? this : elseFn();

        public Result<TValue, TError> And(Result<TValue,TError> r1) => this._isOk ? r1 : this; 

        public Result<UValue, TError> AndThen<UValue>(Func<Result<UValue, TError>> thenFn) => this._isOk  ? thenFn()  : this.Select(val => default(UValue));

        public Maybe<TValue> ToMaybe() => this._isOk ? Maybe<TValue>.Of(this._value) : Maybe<TValue>.Nothing;

        // Apply (ap) - needed?

        public override string ToString() => this._isOk 
            ? $"Ok<{typeof(TValue).Name}>[{this._value}]" 
            : $"Err<{typeof(TError).Name}>[{this._error}]";

        public override bool Equals(object o)
        {
            if (o == null || GetType() != o.GetType())
            {
                return false;
            }

            var r = o as Result<TValue,TError>;
            if (this._isOk)
            {
                return EqualityComparer<TValue>.Default.Equals(this._value,r._value);
            }
            else
            { 
                return EqualityComparer<TError>.Default.Equals(this._error,r._error);
            }
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + typeof(TValue).GetHashCode();
                hash = hash * 23 + typeof(TError).GetHashCode();
                hash = hash * 23 + this._isOk.GetHashCode();
                if (this._isOk)
                {
                    hash = hash + 23 + this._value.GetHashCode();
                }
                else
                {
                    hash = hash + 23 + this._error.GetHashCode();
                }
                return hash;
            }
        }

        public static implicit operator TValue(Result<TValue, TError> result) => 
            result._isOk ? result._value : throw new InvalidOperationException("Invalid conversion to value type.");

        public static implicit operator TError(Result<TValue, TError> result) =>
            !result._isOk ? result._error : throw new InvalidOperationException("Invalid conversion to error type.");

        public static implicit operator Result<TValue,TError>(TValue value) => new Result<TValue,TError>(value, default(TError), true);
        public static implicit operator Result<TValue,TError>(TError error) => new Result<TValue,TError>(default(TValue), error, false);
    }
}