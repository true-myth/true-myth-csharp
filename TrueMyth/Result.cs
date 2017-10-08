using System;

namespace TrueMyth
{
    public interface IResult<TValue, TError>
    {
        TValue UnsafelyUnwrap();
        bool IsOk(IResult<TValue, TError> result);
        bool IsErr(IResult<TValue, TError> result);
        
    }
    
    public class Result
    {
        public static Ok<TValue, TError> Ok<TValue, TError>(TValue value) => new Ok<TValue, TError>(value);
        public static Err<TValue, TError> Err<TValue, TError>(TError error) => new Err<TValue, TError>(error);
        public static bool IsOk<TValue, TError>(IResult<TValue, TError> result) => result.IsOk();
        public static bool IsErr<TValue, TError>(IResult<TValue, TError> result) => result.IsErr();
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
        
        public bool IsOk(IResult<TValue, TError> result) => true;
        public bool IsErr(IResult<TValue, TError> result) => false;
        
        public TValue UnsafelyUnwrap() => _value;
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
        
        public bool IsOk(IResult<TValue, TError> result) => false;
        public bool IsErr(IResult<TValue, TError> result) => true;
        
        public TValue UnsafelyUnwrap() {
            throw new Exception("Tried to `UnsafelyUnwrap(Error)`");
        }
    }
}