using System;
using System.Collections.Generic;
using System.Linq;

namespace TrueMyth
{
    /// <summary>
    /// A static class that provides factory and extension methods for <see cref="Maybe{TValue}"/>.
    /// </summary>
    public static class Maybe
    {
        /// <summary>
        /// This method is a convenience method that is equivalent to <see cref="Maybe{TValue}.Of(TValue)"/>. It constructs a
        /// <c>Maybe&lt;T&gt;</c> from the provided value. This method can take advantage of type inference where <see cref="Maybe{TValue}.Of(TValue)"/>
        /// cannot.
        /// </summary>
        /// <param name="value">The value to be held by the resulting <c>Maybe&lt;T&gt;</c>.false  If <c>value</c> is <c>null</c>,
        /// the resulting <c>Maybe&lt;T&gt;</c> will be a "Nothing".</param>
        /// <typeparam name="T">Any type suported by C♯.</typeparam>
        /// <returns>A <c>Maybe&lt;T&gt;</c> instance representing <c>value</c>.</returns>
        public static Maybe<T> Of<T>(T value) => Maybe<T>.Of(value);

        /// <summary>
        /// This overload, specifically for <c>Nullable&lt;T&gt;</c>, behaves identically to <see cref="Maybe.Of{T}(T)"/>.
        /// </summary>
        public static Maybe<T> Of<T>(Nullable<T> nullable) where T : struct => nullable.HasValue ? Maybe<T>.Of(nullable.Value) : Maybe<T>.Nothing;

        /// <summary>
        /// This method facilitates converting from a <see cref="Result{T,TError}"/> to a <see cref="Maybe{T}"/>.  The difference between this
        /// and <see cref="Maybe.Of{T}(T)"/> is that the resulting <c>Mabye</c> is a <c>Maybe&lt;T&gt;</c> rather than a <c>Maybe&lt;Result&lt;T,TError&gt;&gt;</c>.
        /// If the provided <c>Result</c> is actually an error result (e.g., <c>result.IsErr == true</c>), then the returned <c>Maybe</c> will 
        /// be a "Nothing".
        /// </summary>
        /// <param name="result">The <c>Result&lt;T,TError&gt;</c></param>
        /// <typeparam name="T">The value type of <paramref name="result"/>.</typeparam>
        /// <typeparam name="TError">The error type of <paramref name="result"/>.</typeparam>
        /// <returns>A <c>Maybe&lt;T&gt;</c> representing either a "Just" <c>T</c> or "Nothing", depending on the provided <paramref name="result"/>.</returns>
        public static Maybe<T> From<T,TError>(Result<T, TError> result) => result.UnwrapOrElse(e => Maybe<T>.Nothing);


        /// <summary>
        /// Convert the arguments to a single <c>Maybe&lt;IEnumerable&lt;T&gt;&gt;</c>, but only if all elements of the list are "Just". If any of the elements 
        /// of the list are actually "Nothing", then the result will also be a "Nothing".
        /// </summary>
        /// <param name="maybes">A list of <c>Maybe&lt;T&gt;</c>s to aggregate.</param>
        /// <typeparam name="T">The type represented by the maybe parameters.</typeparam>
        /// <returns>A single <c>Maybe&lt;IEnumerable&lt;T&gt;&gt;</c>.</returns>
        public static Maybe<IEnumerable<T>> MaybeAll<T>(this IEnumerable<Maybe<T>> maybes) => maybes.All(m => m.IsJust)
            ? Maybe<IEnumerable<T>>.Of(maybes.Select(m => (T)m))
            : Maybe<IEnumerable<T>>.Nothing;

        /// <summary>
        /// A convenience method that has the same behavior as <see cref="MaybeAll{T}(IEnumerable{Maybe{T}})"/>.
        /// </summary>
        public static Maybe<IEnumerable<T>> All<T>(params Maybe<T>[] maybes) => maybes.MaybeAll();
        
        /// <summary>
        /// A safe method for finding elements of a list for which the <c>predicate</c> returns <c>true</c>. This method guarantees a valid <c>Maybe&lt;T&gt;</c> result;
        /// it never returns null and never throws an exception.
        /// </summary>
        /// <param name="list">The list to search.</param>
        /// <param name="predicate">A predicate function to execute on each element.</param>
        /// <typeparam name="T">The type of the list elements and of the value of the resulting <c>Maybe&lt;T&gt;</c> instances.</typeparam>
        /// <returns></returns>
        public static Maybe<IEnumerable<T>> MaybeFind<T>(this IEnumerable<T> list, Func<T,bool> predicate)
        {
            var result = list?.Where(t => predicate(t));
            if (result == null || !result.Any())
            {
                return Maybe<IEnumerable<T>>.Nothing;
            }
            else
            {
                return Maybe<IEnumerable<T>>.Of(result);
            }
        }

        /// <summary>
        /// A safe way to retrieve values from key-value collections (like <see cref="Dictionary{TKey,TValue}"/>).  If the requested key does not exist, 
        /// a <c>Maybe&lt;TValue;&gt;.Nothing</c> is returned. Otherwise, a <c>Maybe&lt;TValue&gt;</c> is constructed from the value associated
        /// with <c>key</c>.
        /// </summary>
        /// <param name="collection">The key-value collection from which to retrieve values. Will most often be a <c>Dictionary&lt;TKey,TValue&gt;</c>.</param>
        /// <param name="key">The key to use to retrieve a value.</param>
        /// <typeparam name="TKey">The type of <c>key</c>.</typeparam>
        /// <typeparam name="TValue">The type of the value stored in <c>collection</c> as well as the type stored by the resulting <c>Maybe&lt;TValue&gt;</c>.</typeparam>
        /// <returns>A <c>Maybe&lt;TValue&gt;</c>.Nothing if no value exists associated with the given key; a "Just" otherwise.</returns>
        public static Maybe<TValue> MaybeGet<TKey, TValue>(this ICollection<KeyValuePair<TKey, TValue>> collection, TKey key) => 
            collection.Any(kvp => EqualityComparer<TKey>.Default.Equals(kvp.Key, key)) 
            ? Maybe<TValue>.Of(collection.First(kvp => EqualityComparer<TKey>.Default.Equals(kvp.Key, key)).Value)
            : Maybe<TValue>.Nothing;
        
        /// <summary>
        /// Safely get the first item from a list, returning "Just" the first item if the array has at least one item in it, or "Nothing" if it is empty.
        /// </summary>
        /// <param name="list">The array from which to get the first item.</param>
        /// <typeparam name="TValue">The type of the items of <c>list</c>.</typeparam>
        /// <returns>A <c>Maybe&lt;TValue&gt;</c> of the first item of the list (which might be "Nothing" if the list is empty).</returns>
        public static Maybe<TValue> MaybeFirst<TValue>(this IEnumerable<TValue> list) => list.Any() ? Maybe<TValue>.Of(list.First()) : Maybe<TValue>.Nothing;
        
        /// <summary>
        /// Safely get the last item from a list, returning "Just" the last item if the array has at least one item in it, or "Nothing" if it is empty.
        /// </summary>
        /// <param name="list">The array from which to get the last item.</param>
        /// <typeparam name="TValue">The type of the items of <c>list</c>.</typeparam>
        /// <returns>A <c>Maybe&lt;TValue&gt;</c> of the last item of the list (which might be "Nothing" if the list is empty).</returns>
        public static Maybe<TValue> MaybeLast<TValue>(this IEnumerable<TValue> list) => list.Any() ? Maybe<TValue>.Of(list.Last()) : Maybe<TValue>.Nothing;
        
        /// <summary>
        /// Given a tuple of <c>Maybe</c>s, return a <c>Maybe</c> of the tuple types.  This behaves similarly to <see cref="MaybeAll{T}(IEnumerable{Maybe{T}})"/>.
        /// </summary>
        /// <typeparam name="T">Type of the value represented by the first <c>Maybe</c> in the tuple.</typeparam>
        /// <typeparam name="U">Type of the value reprsented by the second <c>Maybe</c> in the tuple.</typeparam>
        public static Maybe<(T, U)> MaybeAll<T,U>(this Tuple<Maybe<T>,Maybe<U>> tuple) => tuple.Item1.IsJust && tuple.Item2.IsJust 
            ? Maybe<(T,U)>.Of(((T)tuple.Item1, (U)tuple.Item2)) 
            : Maybe<(T,U)>.Nothing;

        /// <summary>
        /// Similar to <see cref="MaybeAll{T,U}(Tuple{Maybe{T},Maybe{U}})"/>, but with a 3-tuple.
        /// </summary>
        public static Maybe<(T,U,V)> MaybeAll<T,U,V>(this Tuple<Maybe<T>,Maybe<U>,Maybe<V>> tuple) => tuple.Item1.IsJust && tuple.Item2.IsJust && tuple.Item3.IsJust
            ? Maybe<(T,U,V)>.Of(((T)tuple.Item1, (U)tuple.Item2, (V)tuple.Item3))
            : Maybe<(T,U,V)>.Nothing;

        /// <summary>
        /// Similar to <see cref="MaybeAll{T,U}(Tuple{Maybe{T},Maybe{U}})"/>, but with a 4-tuple.
        /// </summary>
        public static Maybe<(T,U,V,W)> MaybeAll<T,U,V,W>(this Tuple<Maybe<T>,Maybe<U>,Maybe<V>,Maybe<W>> tuple) => tuple.Item1.IsJust && tuple.Item2.IsJust && tuple.Item3.IsJust && tuple.Item4.IsJust
            ? Maybe<(T,U,V,W)>.Of(((T)tuple.Item1, (U)tuple.Item2, (V)tuple.Item3, (W)tuple.Item4))
            : Maybe<(T,U,V,W)>.Nothing;
    }

    /// <summary>
    ///   <para>
    ///     A <c>Maybe&lt;T&gt;</c> represents a value of type <c>T</c> which may, or may not, be present.
    ///     If the value is present, it is "Just" a value.  If it's absent, it's "Nothing".  This provides a type-safe
    ///     container for dealing with the possibility that there's nothing here — a container you can do many of the
    ///     same things you might with an array — so that you can avoid nasty <c>null</c> checks throughout your codebase.
    ///   </para>
    ///   <para>
    ///     The behavior of this type is checked by the C♯ compiler and bears no runtime overhead other than the very small 
    ///     cost of the container object and some lightweight wrap/unwrap functionality.
    ///     The "Nothing" and "Just" variants are represented internally to the <c>Maybe&lt;T&gt;</c> object and is exposed 
    ///     in several ways.  The most explicit way to check if a <c>Maybe&lt;T&gt;</c> is "Just" or "Nothing" is through the 
    ///     <see cref="IsJust"/> and <see cref="IsNothing"/> properties, respectively.
    ///   </para>
    /// </summary>
    /// <example>
    ///   <code>
    ///     // simple way to construct a Maybe &lt;int&gt;
    ///     var aKnownNumber = Maybe&lt;int&gt;.Just(7);
    ///     
    ///     // once you have it, you can apply methods to it
    ///     var fromMappedJust = aKnownNumber.Select(x => x * 2).Unwrap(0);
    ///     Console.WriteLine(fromMappedJust); // "Just&lt;int&gt;[14]"
    ///     
    ///     // construct a Nothing where you don't have a value to use, but the function requires a value
    ///     var aKnownNothing = Maybe&lt;int&gt;.Nothing;
    ///     
    ///     // the same operations will behave as safely on a Nothing as on a Just:
    ///     var fromMappedNothing = aKnownNothing.Select(x => x * 2).Unwrap(0);
    ///     Console.WriteLine(fromMappedNothing); // "Just&lt;int&gt;[0]"
    /// 
    ///     // construct a Maybe where you don't know where the value will exist or not, using a null check to
    ///     // decide which to construct
    ///     string unknownValue = someFunction();
    ///     var wrappedUnkonwn = unknownValue == null ? Maybe&lt;string&gt;.Just(unknownValue) : Maybe&lt;string&gt;.Nothing;
    /// 
    ///     Console.WriteLine(wrappedUnknown); // either "Just&lt;string&gt;[...]" or "Nothing&lt;string&gt;"
    ///   </code>
    /// </example>
    /// <typeparam name="TValue">Any type supported by the .NET type system.</typeparam>
    public sealed class Maybe<TValue>
    {
        private readonly TValue _value;
        private readonly bool _isJust;

        /// <summary>
        /// Explicit means by which to determine if a <c>Maybe&lt;TValue&gt;</c> is a "Just".  Returns <c>true</c> if "Just"
        /// and <c>false</c> if "Nothing".  Opposite of <see cref="IsNothing"/>.
        /// </summary>
        public bool IsJust => _isJust;

        /// <summary>
        /// Explicit means by which to determin if a <c>Maybe&lt;TValue&gt;</c> is a "Nothing".  Returns <c>true</c> if "Nothing"
        /// and <c>false</c> if "Just".  Exactly the opposite of <see cref="IsJust"/>.
        /// </summary>
        public bool IsNothing => !_isJust;

        private Maybe() 
        {
            _isJust = false;
        }

        private Maybe(TValue value) 
        {
            _value = value;
            _isJust = true;
        }

        /// <summary>
        /// Static factory method for creation of new "Just" <c>Maybe&lt;TValue&gt;</c>s.  If <c>value</c> is a 
        /// reference type and is <c>null</c>, this returns a "Nothing".
        /// </summary>
        /// <param name="value">The value to be held by the <c>Maybe&lt;TValue&gt;</c> instance.  Of type <c>TValue</c>
        /// (see documentation for <see cref="Maybe{TValue}"/>).</param>
        /// <returns>A <c>Maybe&lt;TValue&gt;</c> instance.</returns>
        public static Maybe<TValue> Of(TValue value) => value.Equals(null) ? Nothing : new Maybe<TValue>(value);

        /// <summary>
        /// Static property representing a "Nothing" of type <c>Maybe&lt;TValue&gt;</c>; only constructed once during 
        /// static initialization of the program, so this property is effectively like a constant.
        /// </summary>
        /// <returns>The "Nothing" instance of type <c>Maybe&lt;TValue&gt;</c>.</returns>
        public static Maybe<TValue> Nothing { get; } = new Maybe<TValue>();

        /// <summary>
        /// <para>
        /// You can think of this like a short-circuiting logical "and" operation on a `Mabye` type.  If `this` is a "Just", then
        /// the result is the `andMaybe`.  If `this` is "Nothing", then the result will also be "Nothing" for the destination type
        /// <c>Maybe&lt;UValue&gt;</c>.
        /// </para>
        /// <para>
        /// This is useful when you have another `Maybe` value you want to provide <em>iff</em> you have a "Just" — that is, when you
        /// need to make sure that if you have "Nothing", whatever else you're handing a <c>Maybe</c> <em>also</em> gets a "Nothing".
        /// </para>
        /// <para>
        /// Other things to note:
        /// <list>
        ///   <item>The return type may be different than the type of `this`.</item>
        ///   <item>Unlike in <see cref="Map{UValue}(Func{TValue,UValue})"/> the original <c>Maybe&lt;TValue&gt;</c> is not involved in constructing the new <c>Maybe&lt;UValue&gt;</c>.</item>
        /// </list>
        /// </para>
        /// </summary>
        /// <param name="andMaybe"></param>
        /// <typeparam name="UValue"></typeparam>
        /// <returns></returns>
        public Maybe<UValue> And<UValue>(Maybe<UValue> andMaybe) => this._isJust ? andMaybe : Maybe<UValue>.Nothing;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="thenFn"></param>
        /// <typeparam name="UValue"></typeparam>
        /// <returns></returns>
        public Maybe<UValue> And<UValue>(Func<TValue,Maybe<UValue>> thenFn) => this._isJust ? thenFn(this._value) : Maybe<UValue>.Nothing;
        
        // Apply?
        
        // Map
        public Maybe<UValue> Map<UValue>(Func<TValue,UValue> mapFn) => this._isJust ? Maybe<UValue>.Of(mapFn(this._value)) : Maybe<UValue>.Nothing;
        
        // MapOr
        public Maybe<UValue> Map<UValue>(Func<TValue,UValue> mapFn, UValue defaultValue) => this._isJust ? Maybe<UValue>.Of(mapFn(this._value)) : Maybe<UValue>.Of(defaultValue);

        // MapOrElse
        public Maybe<UValue> Map<UValue>(Func<TValue,UValue> mapFn, Func<UValue> elseFn) => this._isJust ? Maybe<UValue>.Of(mapFn(this._value)) : Maybe<UValue>.Of(elseFn());
        
        public T Match<T>(Func<TValue,T> just, Func<T> nothing) => this._isJust ? just(this._value) : nothing();

        public Maybe<TValue> Or(Maybe<TValue> maybe) => this._isJust ? this : maybe;

        // OrElse
        public Maybe<TValue> Or(Func<Maybe<TValue>> elseFn) => this._isJust ? this : elseFn();
        
        // toOkOrElseErr
        public Result<TValue, TError> ToResult<TError>(Func<TError> errFn) => this._isJust ? Result<TValue,TError>.Ok(this._value) : Result<TValue,TError>.Err(errFn());

        // toOkOrErr
        public Result<TValue, TError> ToResult<TError>(TError error) => this._isJust ? Result<TValue, TError>.Ok(this._value) : Result<TValue, TError>.Err(error);

        // unsafelyUnwrap
        public TValue UnsafelyUnwrap() => this._isJust ? this._value : throw new InvalidOperationException($"Invalid attempt to unwrap {GetType().Name}.Nothing");
        
        public TValue Unwrap(TValue defaultValue) => this._isJust ? this._value : defaultValue;
        
        public TValue Unwrap(Func<TValue> elseFn) => this._isJust ? this._value : elseFn();

        // #### object stuff
        public override string ToString() => this._isJust
            ? $"Just<{typeof(TValue)}>[{this._value}]"
            : $"Nothing<{typeof(TValue)}>[]";

        public override bool Equals(object obj)
        {   
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            
            var m = obj as Maybe<TValue>;
            if (this._isJust != m._isJust)
                return false;

            if (!this._isJust && !m._isJust)
                return true;
            
            return EqualityComparer<TValue>.Default.Equals(this._value, m._value);
        }
        
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + typeof(TValue).GetHashCode();
                hash = hash * 23 + this._isJust.GetHashCode();
                if (this._isJust)
                {
                    hash = hash * 23 + this._value.GetHashCode();
                }
                return hash;
            }
        }

        public static implicit operator TValue(Maybe<TValue> maybe) => maybe.UnsafelyUnwrap();
        public static implicit operator Maybe<TValue>(TValue value) => new Maybe<TValue>(value);

        // TODO: implicit for Nothing?
    }
}