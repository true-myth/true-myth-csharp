using System;
using System.Collections.Generic;

namespace TrueMyth
{
    /// <summary>
    /// <para>A <c>Result&lt;TValue,TError&gt;</c> is a type representing the value result of an operation which may fail,
    /// with a successful value type of <c>TValue</c> or an error type of <c>TError</c> (pun intended!).  If
    /// the value is present, it is "Ok", and if it's absent, it's "Err". There are several ways to check if a <c>Result</c>
    /// is Ok or Err, but the most direct and explicit are the <see cref="IsOk"/> and <see cref="IsErr"/> properties. 
    /// </para>
    /// <para>
    /// This provides a type-safe container for dealing with the possibility that an error occurred, without needing to scatter
    /// <c>try</c>/<c>catch</c> blocks throughout your codebase. This has two major advantages:
    /// <list type="bullet">
    ///   <item>
    ///     <description>
    ///     You <em>know</em> when an item may have a failure case, unlike cases in which exceptions are thrown (which
    ///     may be thrown from any function with no warning and no help from the type system).
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///     The error scenario is a first-class citizen, and the provided helper functions and methods allow you
    ///     to deal with the type in much the same way as you might an array — transforming values if present,
    ///     or dealing with errors instead of necessary.
    ///     </description>
    ///   </item>
    /// </list>
    /// </para>
    /// </summary>
    /// <example>
    /// To make this concrete, let's look at an example.see  Without TrueMyth, you might have the following
    /// C♯:
    /// <code>
    /// int MightSucceed(bool doesSucceed)
    /// {
    ///     if (!doesSucceed) throw new Exception("hey guess what? it didn't succeed.see");
    /// 
    ///     return 42;
    /// }
    /// 
    /// int Main(string[] args)
    /// {
    ///     var doubleTheAnswer = MightSucceed(true) * 2;
    ///     Console.WriteLine(doubleTheAnswer); // 84; this is fine
    /// 
    ///     var doubleAnError = MightSucceed(false) * 2;
    ///     Console.Write(doubleAnError); // oops!  we never even get here.
    /// }
    /// </code>
    /// If we wanted to <em>handle</em> that error, we'd need to first of all know that the function could
    /// throw an error. Assuming we knew that — progbably we'd figure it out via painful discovery at runtime —
    /// then we'd need to wrap it up in a <c>try</c>/<c>catch</c> block:
    /// <code>
    /// int Main(string[] args)
    /// {
    ///     try
    ///     {
    ///         var doubleTheAnswer = MightSucceed(true) * 2;
    ///         Console.WriteLine(doubleTheAnswer); // 84; this is fine
    /// 
    ///         var doubleAnError = MightSucceed(false) * 2;
    ///         Console.WriteLine(doubleAnError);
    ///     }
    ///     catch(Exception exn)
    ///     {
    ///         Console.WriteLine(exn.Message);
    ///     }
    /// }
    /// </code>
    /// This is a pain to work with!
    /// 
    /// The next thing we might try is returning an error code and mutating an object passed in (e.g. 
    /// <c>bool TryMightSucceed(bool doesSucceed, out int result)</c>). But that has a few problems:
    /// <list type="bullet">
    ///   <item>
    ///     <description>
    ///     You have to mutate an object. This doesn't work for simple items like numbers, and it can also
    ///     be pretty unexpected behavior at times — you want to <em>know</em> when something is going to change,
    ///     and mutating freely throughout a library or application makes that impossible.
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///     You have to make sure to actually check the return code to make sure it's valid. In theory,
    ///     we're all disciplined enough to always do that. In practice, we often end up reasoning,
    ///     <em>Well, this particular call can never fail... </em> (but of course, it probably can, just not 
    ///     in the way we expect).
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///     We don't have a good way to return a <em>reason</em> for the error. We end up needing to introduce
    ///     another parameter, designed to be mutated, to make sure that's possible.
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///     Even if you go to all the trouble of doing all that, you need to make sure — every time — that you
    ///     use only the error value if the return code specified an error, and only the success value if the
    ///     return code specified that it succeeded.
    ///     </description>
    ///   </item>
    /// </list>
    /// Our way out is <c>Result&lt;TValue,TError&gt;</c>. It lets us just return one thing from a function, 
    /// which encapsulates the possiblility of failure in the very thing we return. We get:
    /// <list>
    ///   <item>
    ///     <description>
    ///     the simplicity of jsut dealing with the return value of a function (no <c>try</c>/<c>catch</c>
    ///     to worry about!)
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///     the ease of expressing an error we got without throwing an exception
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///     the explicitness about success or failure that we got with a return code
    ///     </description>
    ///   </item>
    /// </list>
    /// Here's what that same example from above would look like using <c>Result</c>:
    /// <code>
    /// Result&lt;int,string&gt; MightSucceed(bool doesSucceed) => 
    ///     doesSucceed 
    ///         ? Result&lt;int,string&gt;.Ok(42) 
    ///         : Result&lt;int,string&gt;.Err("something went wrong!");
    /// 
    /// int Main(string[] args)
    /// {
    ///     int Double(int x) = x * 2;
    /// 
    ///     var doubleTheAnswer = MightSucceed(true).Select(Double);
    ///     Console.WriteLine(doubleTheAnswer); // Ok&lt;int,string&gt;[84]
    /// 
    ///     var doubleAnErr = MightSucceed(false).Select(Double);
    ///     Console.WriteLine(doubleAnErr); // Err&lt;int,string&gt;[something went wrong]
    /// }
    /// </code>
    /// Note that if we tried to call <c>MightSucceed(true)*2</c> here, we'd get a type error — this wouldn't make it 
    /// past the compile step.
    /// </example>
    /// <typeparam name="TValue">The value type for "Ok" values.</typeparam>
    /// <typeparam name="TError">The value type for "Err" values.</typeparam>
    public sealed class Result<TValue, TError>
    {
        private readonly TValue _value;
        private readonly TError _error;
        private readonly bool _isOk;

        /// <summary>
        /// Is this <c>Result</c> an "Ok"? This property is <c>true</c> if so.
        /// </summary>
        public bool IsOk => _isOk;

        /// <summary>
        /// This is merely the reverse of <see cref="IsOk"/>; if <c>this</c> is an "Err", then
        /// the property is <c>true</c>.
        /// </summary>
        public bool IsErr => !_isOk;

        private Result() {}

        private Result(TValue value, TError error, bool isOk)
        {
            _value = value;
            _error = error;
            _isOk = isOk;
        }

        /// <summary>
        /// A factory method for creating "Ok" <c>Result</c> instances.
        /// <note><c>null</c> is allowed by the type signature, but it is highly recommended
        /// to use <see cref="Maybe{TValue}"/> rather than a <c>null</c> as a result.
        /// </note>
        /// </summary>
        /// <param name="value">The success value wrapped by the <c>Result</c></param>
        public static Result<TValue, TError> Ok(TValue value) => new Result<TValue, TError>(value, default(TError), true);

        /// <summary>
        /// A factory method for creating "Err" <c>Result</c> instances.
        /// </summary>
        /// <param name="err">The error value wrapped by the <c>Result</c></param>
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