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
        /// A convenience method that has the same behavior as <see cref="AsMaybe{T}(IEnumerable{Maybe{T}})"/>.
        /// </summary>
        public static Maybe<IEnumerable<T>> All<T>(params Maybe<T>[] maybes) => maybes.AsMaybe();

        /// <summary>
        /// Convert the arguments to a single <c>Maybe&lt;IEnumerable&lt;T&gt;&gt;</c>, but only if all elements of the list are <b>Just</b>. If any of the elements 
        /// of the list are actually <b>Nothing</b>, then the result will also be a <b>Nothing</b>.
        /// </summary>
        /// <param name="maybes">A list of <c>Maybe&lt;T&gt;</c>s to aggregate.</param>
        /// <typeparam name="T">The type represented by the maybe parameters.</typeparam>
        /// <returns>A single <c>Maybe&lt;IEnumerable&lt;T&gt;&gt;</c>.</returns>
        public static Maybe<IEnumerable<T>> AsMaybe<T>(this IEnumerable<Maybe<T>> maybes) => maybes.All(m => m.IsJust)
            ? Maybe<IEnumerable<T>>.Of(maybes.Select(m => (T)m))
            : Maybe<IEnumerable<T>>.Nothing;

        /// <summary>
        /// Given a tuple of <c>Maybe</c>s, return a <c>Maybe</c> of the tuple types.  This behaves similarly to <see cref="AsMaybe{T}(IEnumerable{Maybe{T}})"/>.
        /// </summary>
        /// <typeparam name="T">Type of the value represented by the first <c>Maybe</c> in the tuple.</typeparam>
        /// <typeparam name="U">Type of the value reprsented by the second <c>Maybe</c> in the tuple.</typeparam>
        public static Maybe<(T, U)> AsMaybe<T,U>(this Tuple<Maybe<T>,Maybe<U>> tuple) => tuple.Item1.IsJust && tuple.Item2.IsJust 
            ? Maybe<(T,U)>.Of(((T)tuple.Item1, (U)tuple.Item2)) 
            : Maybe<(T,U)>.Nothing;

        /// <summary>
        /// Similar to <see cref="AsMaybe{T,U}(Tuple{Maybe{T},Maybe{U}})"/>, but with a 3-tuple.
        /// </summary>
        public static Maybe<(T,U,V)> AsMaybe<T,U,V>(this Tuple<Maybe<T>,Maybe<U>,Maybe<V>> tuple) => tuple.Item1.IsJust && tuple.Item2.IsJust && tuple.Item3.IsJust
            ? Maybe<(T,U,V)>.Of(((T)tuple.Item1, (U)tuple.Item2, (V)tuple.Item3))
            : Maybe<(T,U,V)>.Nothing;

        /// <summary>
        /// Similar to <see cref="AsMaybe{T,U}(Tuple{Maybe{T},Maybe{U}})"/>, but with a 4-tuple.
        /// </summary>
        public static Maybe<(T,U,V,W)> AsMaybe<T,U,V,W>(this Tuple<Maybe<T>,Maybe<U>,Maybe<V>,Maybe<W>> tuple) => tuple.Item1.IsJust && tuple.Item2.IsJust && tuple.Item3.IsJust && tuple.Item4.IsJust
            ? Maybe<(T,U,V,W)>.Of(((T)tuple.Item1, (U)tuple.Item2, (V)tuple.Item3, (W)tuple.Item4))
            : Maybe<(T,U,V,W)>.Nothing;


        /// <summary>
        /// Converts a <c>Maybe&lt;T&gt;</c> to a <see cref="Nullable{T}"/>.  If <c>this</c> is a **Just**, then the result will be a <c>Nullable&lt;T&gt;</c>
        /// where <c>HasValue</c> is <c>true</c> and whose value is the value of the maybe.  If <c>this</c> is a **Nothing**, then the result will be a <c>Nullable&lt;T&gt;</c> where <c>HasValue</c> is
        /// <c>false</c> and whose value is <c>null</c>.
        /// </summary>
        public static T? AsNullable<T>(this Maybe<T> maybe) where T : struct => maybe.MapReturn(t => new Nullable<T>(t), (T?)null);

        /// <summary>
        /// This method facilitates converting from a <see cref="Result{T,TError}"/> to a <see cref="Maybe{T}"/>.  The difference between this
        /// and <see cref="Maybe.Of{T}(T)"/> is that the resulting <c>Mabye</c> is a <c>Maybe&lt;T&gt;</c> rather than a <c>Maybe&lt;Result&lt;T,TError&gt;&gt;</c>.
        /// If the provided <c>Result</c> is actually an error result (e.g., <c>result.IsErr == true</c>), then the returned <c>Maybe</c> will 
        /// be a <b>Nothing</b>.
        /// </summary>
        /// <param name="result">The <c>Result&lt;T,TError&gt;</c></param>
        /// <typeparam name="T">The value type of <paramref name="result"/>.</typeparam>
        /// <typeparam name="TError">The error type of <paramref name="result"/>.</typeparam>
        /// <returns>A <c>Maybe&lt;T&gt;</c> representing either a <b>Just</b> <c>T</c> or <b>Nothing</b>, depending on the provided <paramref name="result"/>.</returns>
        public static Maybe<T> From<T,TError>(Result<T, TError> result) => result.ToMaybe();

        /// <summary>
        /// Creates a new <c>Maybe&lt;T&gt;</c> from a <see cref="Nullable{T}"/>. If the <c>Nullable&lt;T&gt;</c> is <c>null</c>, then the resulting
        /// maybe will be a **Nothing**; otherwise, the resulting maybe will be a **Just** and its value will be the value of the <c>Nullable&lt;T&gt;</c>.
        /// </summary>
        public static Maybe<T> From<T>(T? nullable) where T : struct => nullable.HasValue ? Maybe.Of(nullable.Value) : Maybe<T>.Nothing;

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
        /// Safely get the first item from a list, returning <b>Just</b> the first item if the array has at least one item in it, or <b>Nothing</b> if it is empty.
        /// </summary>
        /// <param name="list">The array from which to get the first item.</param>
        /// <typeparam name="TValue">The type of the items of <c>list</c>.</typeparam>
        /// <returns>A <c>Maybe&lt;TValue&gt;</c> of the first item of the list (which might be <b>Nothing</b> if the list is empty).</returns>
        public static Maybe<TValue> MaybeFirst<TValue>(this IEnumerable<TValue> list) => list.Any() ? Maybe<TValue>.Of(list.First()) : Maybe<TValue>.Nothing;

        /// <summary>
        /// A safe way to retrieve values from key-value collections (like <see cref="Dictionary{TKey,TValue}"/>).  If the requested key does not exist, 
        /// a <c>Maybe&lt;TValue;&gt;.Nothing</c> is returned. Otherwise, a <c>Maybe&lt;TValue&gt;</c> is constructed from the value associated
        /// with <c>key</c>.
        /// </summary>
        /// <param name="collection">The key-value collection from which to retrieve values. Will most often be a <c>Dictionary&lt;TKey,TValue&gt;</c>.</param>
        /// <param name="key">The key to use to retrieve a value.</param>
        /// <typeparam name="TKey">The type of <c>key</c>.</typeparam>
        /// <typeparam name="TValue">The type of the value stored in <c>collection</c> as well as the type stored by the resulting <c>Maybe&lt;TValue&gt;</c>.</typeparam>
        /// <returns>A <c>Maybe&lt;TValue&gt;</c>.Nothing if no value exists associated with the given key; a <b>Just</b> otherwise.</returns>
        public static Maybe<TValue> MaybeGet<TKey, TValue>(this ICollection<KeyValuePair<TKey, TValue>> collection, TKey key) => 
            collection.Any(kvp => EqualityComparer<TKey>.Default.Equals(kvp.Key, key)) 
            ? Maybe<TValue>.Of(collection.First(kvp => EqualityComparer<TKey>.Default.Equals(kvp.Key, key)).Value)
            : Maybe<TValue>.Nothing;

        /// <summary>
        /// Safely get the last item from a list, returning <b>Just</b> the last item if the array has at least one item in it, or <b>Nothing</b> if it is empty.
        /// </summary>
        /// <param name="list">The array from which to get the last item.</param>
        /// <typeparam name="TValue">The type of the items of <c>list</c>.</typeparam>
        /// <returns>A <c>Maybe&lt;TValue&gt;</c> of the last item of the list (which might be <b>Nothing</b> if the list is empty).</returns>
        public static Maybe<TValue> MaybeLast<TValue>(this IEnumerable<TValue> list) => list.Any() ? Maybe<TValue>.Of(list.Last()) : Maybe<TValue>.Nothing;

        /// <summary>
        /// This method is a convenience method that is equivalent to <see cref="Maybe{TValue}.Of(TValue)"/>. It constructs a
        /// <c>Maybe&lt;T&gt;</c> from the provided value. This method can take advantage of type inference where <see cref="Maybe{TValue}.Of(TValue)"/>
        /// cannot.
        /// </summary>
        /// <param name="value">The value to be held by the resulting <c>Maybe&lt;T&gt;</c>.false  If <c>value</c> is <c>null</c>,
        /// the resulting <c>Maybe&lt;T&gt;</c> will be a <b>Nothing</b>.</param>
        /// <typeparam name="T">Any type suported by C♯.</typeparam>
        /// <returns>A <c>Maybe&lt;T&gt;</c> instance representing <c>value</c>.</returns>
        public static Maybe<T> Of<T>(T value) => Maybe<T>.Of(value);

        /// <summary>
        /// This overload, specifically for <c>Nullable&lt;T&gt;</c>, behaves identically to <see cref="Maybe.Of{T}(T)"/>.
        /// </summary>
        public static Maybe<T> Of<T>(Nullable<T> nullable) where T : struct => nullable.HasValue ? Maybe<T>.Of(nullable.Value) : Maybe<T>.Nothing;
    }

    /// <summary>
    ///   <para>
    ///     A <c>Maybe&lt;T&gt;</c> represents a value of type <c>T</c> which may, or may not, be present. If the value
    ///     is present, it is <b>Just</b> a value.  If it's absent, it's <b>Nothing</b>.  This provides a type-safe container
    ///     for dealing with the possibility that there's nothing here — a container you can do many of the same things
    ///     you might with an array — so that you can avoid nasty <c>null</c> checks throughout your codebase.
    ///   </para>
    ///   <para>
    ///     The behavior of this type is checked by the C♯ compiler and bears no runtime overhead other than the very
    ///     small cost of the container object and some lightweight wrap/unwrap functionality. The <b>Nothing</b> and
    ///     <b>Just</b> variants are represented internally to the <c>Maybe&lt;T&gt;</c> object and is exposed in several
    ///     ways.  The most explicit way to check if a <c>Maybe&lt;T&gt;</c> is <b>Just</b> or <b>Nothing</b> is through the 
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
    ///     // construct a Nothing where you don't have a value to use, but the function 
    ///     // requires a value 
    ///     var aKnownNothing = Maybe&lt;int&gt;.Nothing;
    ///
    ///     // the same operations will behave as safely on a Nothing as on a Just: 
    ///     var fromMappedNothing = aKnownNothing.Select(x => x * 2).Unwrap(0); 
    ///     Console.WriteLine(fromMappedNothing); // "Just&lt;int&gt;[0]"
    ///
    ///     // construct a Maybe where you don't know where the value will exist or not, using a 
    ///     // null check to decide which to construct 
    ///     string unknownValue = someFunction(); 
    ///     var wrappedUnkonwn = unknownValue == null 
    ///         ? Maybe&lt;string&gt;.Just(unknownValue) 
    ///         : Maybe&lt;string&gt;.Nothing;
    ///     Console.WriteLine(wrappedUnknown);// either "Just&lt;string&gt;[...]" or "Nothing&lt;string&gt;" 
    ///   </code>
    /// </example>
    /// <typeparam name="TValue">Any type supported by the .NET type system.</typeparam>
    public sealed class Maybe<TValue> : IComparable, IComparable<Maybe<TValue>>
    {
        #region Private Fields

        private readonly TValue _value;
        private readonly bool _isJust;

        #endregion

        #region Public Properties

        /// <summary>
        /// Explicit means by which to determine if a <c>Maybe&lt;TValue&gt;</c> is a <b>Just</b>.  Returns <c>true</c> if <b>Just</b>
        /// and <c>false</c> if <b>Nothing</b>.  Opposite of <see cref="IsNothing"/>.
        /// </summary>
        public bool IsJust => _isJust;

        /// <summary>
        /// Explicit means by which to determin if a <c>Maybe&lt;TValue&gt;</c> is a
        /// <b>Nothing</b>.  Returns <c>true</c> if <b>Nothing</b> and <c>false</c> if
        /// <b>Just</b>. Exactly the opposite of <see cref="IsJust"/>.
        /// </summary>
        public bool IsNothing => !_isJust;

        /// <summary>
        /// Static property representing a <b>Nothing</b> of type <c>Maybe&lt;TValue&gt;</c>; only constructed once during 
        /// static initialization of the program, so this property is effectively like a constant.
        /// </summary>
        /// <returns>The <b>Nothing</b> instance of type <c>Maybe&lt;TValue&gt;</c>.</returns>
        public static Maybe<TValue> Nothing { get; } = new Maybe<TValue>();

        #endregion

        #region Constructors

        private Maybe() 
        {
            _isJust = false;
        }

        private Maybe(TValue value) 
        {
            _value = value;
            _isJust = true;
        }

        #endregion

        #region Public Instance Methods
        /// <summary>
        /// <para>
        /// You can think of this like a short-circuiting logical "and" operation on a <c>Mabye</c> type.  If <c>this</c> is a <b>Just</b>, then
        /// the result is the <c>andMaybe</c>.  If <c>this</c> is <b>Nothing</b>, then the result will also be <b>Nothing</b> for the destination type
        /// <c>Maybe&lt;UValue&gt;</c>.
        /// </para>
        /// <para>
        /// This is useful when you have another <c>Maybe</c> value you want to provide <em>iff</em> you have a <b>Just</b> — that is, when you
        /// need to make sure that if you have <b>Nothing</b>, whatever else you're handing a <c>Maybe</c> <em>also</em> gets a <b>Nothing</b>.
        /// </para>
        /// <para>
        /// Other things to note:
        /// <list>
        ///   <item>The return type may be different than the type of <c>this</c>.</item>
        ///   <item>Unlike in <see cref="Select{UValue}(Func{TValue,UValue})"/> the original <c>Maybe&lt;TValue&gt;</c> is not involved in constructing the new <c>Maybe&lt;UValue&gt;</c>.</item>
        /// </list>
        /// </para>
        /// </summary>
        /// <param name="andMaybe"></param>
        /// <typeparam name="UValue"></typeparam>
        public Maybe<UValue> And<UValue>(Maybe<UValue> andMaybe) => this._isJust ? andMaybe : Maybe<UValue>.Nothing;
        
        /// <summary>
        /// This work similarly to <see cref="And{UValue}(Maybe{UValue})"/>, but instead of providing a maybe that will be returned when <c>this</c> is <b>Just</b>, 
        /// you provide a function that returns a <c>Maybe</c>.
        /// </summary>
        /// <param name="bindFn">Function that returns a <c>Maybe&lt;UValue&gt;</c></param>
        /// <typeparam name="UValue">Any type supported by C♯; it needn't be the same as <c>TValue</c>.</typeparam>
        public Maybe<UValue> AndThen<UValue>(Func<TValue,Maybe<UValue>> bindFn) => this._isJust ? bindFn(this._value) : Maybe<UValue>.Nothing;

        /// <summary>
        /// Map over a <c>Maybe</c> instance.static  This applies the function to the wrapped value if the instance is <b>Just</b> and returns <b>Nothing</b>
        /// if the instance is <b>Nothing</b>.  This respects types, so if <c>this</c> is of type <c>Maybe&lt;int&gt;</c> and you provide a <c>mapFn</c> of type <c>Func&lt;double&gt;</c>,
        /// the type of <c>Maybe</c> that will be returned will be <c>Maybe&lt;double&gt;</c>, irrespective of <c>this</c> being <b>Just</b> or <b>Nothing</b>.
        /// </summary>
        /// <param name="mapFn">The mapping function to be applied to the wrapped value.</param>
        /// <typeparam name="UValue">The type of the resulting <c>Maybe</c> value.</typeparam>
        public Maybe<UValue> Map<UValue>(Func<TValue,UValue> mapFn) => this._isJust ? Maybe<UValue>.Of(mapFn(this._value)) : Maybe<UValue>.Nothing;

        /// <summary>
        /// Map over a <c>Maybe&lt;TValue&gt;</c> instance and get out the value if it's a <b>Just</b>, or return a default value if
        /// it's a <b>Nothing</b>>. It differs from <see cref="Map{UValue}(Func{TValue,UValue})"/> in that the result is the
        /// value type itself (or a mapped type).
        /// </summary>
        /// <param name="mapFn">The mapping function to be applied to the wrapped value.</param>
        /// <param name="defaultValue">A fallback value of type <c>UValue</c> to be returned in the case that
        /// <c>this</c> is <b>Nothing</b>.</param>
        /// <typeparam name="UValue">The type of the result of the mapping function; any type supported by C♯ can be
        /// used here; it needn't be different than <c>TValue</c>.</typeparam>
        /// <returns>A <c>UValue</c> as computed by <c>mapFn</c> if <c>this</c> is <b>Just</b>; otherwise, the
        /// <c>defaultValue</c>.</returns>
        public UValue MapReturn<UValue>(Func<TValue,UValue> mapFn, UValue defaultValue) => this._isJust ? mapFn(this._value) : defaultValue;

        /// <summary>
        /// Provides the same basic functionality as <see cref="UnwrapOrElse(Func{TValue})"/>, but instead of simply unwrapping the value if it is <b>Just</b> and applyiung a value to genearte
        /// the same default type if it is <b>Nothing</b>, lets you supply functions which may transform the wrapped type if it is <b>Just</b> or get a default value for <b>Nothing</b>.
        /// </summary>
        /// <param name="just">The function to apply if <c>this</c> is <b>Just</b>.</param>
        /// <param name="nothing">The function to apply if <c>this</c> is <b>Nothing</b>.</param>
        /// <example>
        /// var maybeValue = fromSomeMethod();
        /// var stringValue = maybeValue.Match(
        ///     just: val => val.ToString(),
        ///     nothing: val => "nothing"
        /// );
        /// </example>
        public UValue Match<UValue>(Func<TValue,UValue> just, Func<UValue> nothing) => this._isJust ? just(this._value) : nothing();

        /// <summary>
        /// Static factory method for creation of new <b>Just</b> <c>Maybe&lt;TValue&gt;</c>s.  If <c>value</c> is a 
        /// reference type and is <c>null</c>, this returns a <b>Nothing</b>.
        /// </summary>
        /// <param name="value">The value to be held by the <c>Maybe&lt;TValue&gt;</c> instance.  Of type <c>TValue</c>
        /// (see documentation for <see cref="Maybe{TValue}"/>).</param>
        /// <returns>A <c>Maybe&lt;TValue&gt;</c> instance.</returns>
        public static Maybe<TValue> Of(TValue value) => value == null ? Nothing : new Maybe<TValue>(value);

        /// <summary>
        /// Provide a fallback for <c>this</c>.  Behaves like a logical "or": if the <c>Maybe</c> is <b>Just</b>, returns that; otherwise,
        /// returns the provided <c>maybe</c>.
        /// </summary>
        /// <param name="maybe">Fallback value.</param>
        /// <returns>If <c>this</c> is <b>Just</b>, return <c>this</c>; otherwise return <c>maybe</c>.</returns>
        public Maybe<TValue> Or(Maybe<TValue> maybe) => this._isJust ? this : maybe;

        /// <summary>
        /// Like <see cref="Or(Maybe{TValue})"/>, but using a function to construct the fallback <c>Maybe</c>.
        /// </summary>
        /// <param name="elseFn">Function used to construct fallback <c>Maybe</c>.</param>
        /// <returns>If <c>this</c> is <b>Just</b>, returns <c>this</c>.  Otherwise, returns result of <c>elseFn</c>.</returns>
        public Maybe<TValue> OrElse(Func<Maybe<TValue>> elseFn) => this._isJust ? this : elseFn();

        /// <summary>
        /// An alias for <see cref="Map{UValue}(Func{TValue,UValue})"/>; provided for familiarity for C♯ programmers.
        /// </summary>
        /// <param name="mapFn">The mapping function to be applied to the wrapped value.</param>
        /// <typeparam name="UValue">The type of the resulting <c>Maybe</c> value.</typeparam>
        public Maybe<UValue> Select<UValue>(Func<TValue,UValue> mapFn) => Map(mapFn);

        /// <summary>
        /// Transform the <c>Maybe</c> into a <see cref="Result{TValue,TError}"/>, using the wrapped value as the <b>Ok</b> value if <b>Just</b>; otherwise using the supplied 
        /// <c>error</c> value for <b>Err</b>.
        /// </summary>
        /// <param name="error">The error value to use if the <c>Maybe</c> is <b>Nothing</b>.</param>
        /// <typeparam name="TError">The wrapped value type.</typeparam>
        public Result<TValue, TError> ToResult<TError>(TError error) => this._isJust ? Result<TValue, TError>.Ok(this._value) : Result<TValue, TError>.Err(error);

        /// <summary>
        /// Very similar to <see cref="ToResult{TError}(TError)"/>, but using a function to construct the error result.
        /// </summary>
        public Result<TValue, TError> ToResult<TError>(Func<TError> errFn) => this._isJust ? Result<TValue,TError>.Ok(this._value) : Result<TValue,TError>.Err(errFn());

        /// <summary>
        /// Get the <c>TValue</c> value out of the <c>Maybe&lt;TValue&gt;</c>. Returns the content of a <b>Just</b>, but <em>throws if the <c>Maybe</c> is <b>Nothing</b></em>.
        /// Prefer to use <see cref="UnwrapOr(TValue)"/> or <see cref="UnwrapOrElse(Func{TValue})"/>.
        /// </summary>
        public TValue UnsafelyUnwrap() => this._isJust ? this._value : throw new InvalidOperationException($"Invalid attempt to unwrap {GetType().Name}.Nothing as {typeof(TValue).Name}");
        
        /// <summary>
        /// Safely get the <c>TValue</c> value out of the <c>Maybe&lt;TValue&gt;</c>. Returns the content of <b>Just</b> or <c>defaultValue</c> if <c>this</c> is <b>Nothing</b>.
        /// This is the recommended way to get a value out of a <c>Maybe</c> most of the time.
        /// </summary>
        public TValue UnwrapOr(TValue defaultValue) => this._isJust ? this._value : defaultValue;
        
        /// <summary>
        /// Safely get the value out of a <c>Maybe</c> by returning the wrapped value if it is <b>Just</b>, oir by applying
        /// <c>elseFn</c> if it's <b>Nothing</b>.  This is useful when you need to <em>generate</em> a value (e.g. by using current values in the environment
        /// — whether preloaded or by local closure) instead of having a default value available as in <see cref="UnwrapOr(TValue)"/>.
        /// </summary>
        public TValue UnwrapOrElse(Func<TValue> elseFn) => this._isJust ? this._value : elseFn();

        #endregion

        #region Object Overrides

        /// <summary>
        /// Produces a string format like the following: "Just&lt;TValue&gt;[value]" or "Nothing&lt;TValue&gt;".
        /// </summary>
        public override string ToString() => this._isJust
            ? $"Just<{typeof(TValue)}>[{this._value}]"
            : $"Nothing<{typeof(TValue)}>";

        /// <exclude/>
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
        
        /// <exclude/>
        public override int GetHashCode()
        {
            unchecked
            {
                const int prime = 29;
                int hash = 17;
                hash = hash * prime + typeof(TValue).GetHashCode();
                hash = hash * prime + this._isJust.GetHashCode();
                if (this._isJust)
                {
                    hash = hash * prime + this._value.GetHashCode();
                }
                return hash;
            }
        }

        #endregion

        #region IComparable implementation

        /// <exclude/>
        public int CompareTo(Maybe<TValue> otherMaybe)
        {
            if (otherMaybe == null) 
            {
                return 1;
            }

            if (object.ReferenceEquals(this, otherMaybe))
            {
                return 0;
            }

            if (this.IsNothing)
            {
                return otherMaybe.IsJust ? -1 : 0;
            } 
            else
            {
                if (otherMaybe.IsNothing)
                {
                    return 1;
                }
                else
                {
                    if (typeof(IComparable).IsAssignableFrom(typeof(TValue)))
                    {
                        var justThis = UnsafelyUnwrap() as IComparable;
                        var justThat = otherMaybe.UnsafelyUnwrap();
                        return justThis.CompareTo(justThat);
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
        }

        /// <exclude/>
        public int CompareTo(object obj)
        {
            if (GetType() != obj.GetType())
            {
                throw new ArgumentException($"Parameter of different type: {obj.GetType()}", nameof(obj));
            }

            return CompareTo((Maybe<TValue>)obj);
        }

        #endregion

        #region Public Static Methods & Operators

        /// <summary>
        /// Equivalent of <see cref="UnsafelyUnwrap"/>.  Follows usual C♯ semantics of throwing 
        /// an exception at runtime if the conversion is invalid.
        /// </summary>
        public static explicit operator TValue(Maybe<TValue> maybe) => maybe.UnsafelyUnwrap();
        
        /// <summary>
        /// Constructs a <c>Maybe&lt;TValue&gt;</c> explicitly. Equivalent of <see cref="Maybe{TValue}.Of(TValue)"/>.
        /// </summary>
        /// <example>
        /// This is useful for reducing ceremony when using TrueMyth, particularly when refactoring into existing
        /// code.  Without TrueMyth at all, you might have a method like this:
        /// <code>
        /// string GetTranslation(string sourceText)
        /// {
        ///     string result = null;
        ///     result = translationService.Lookup(sourceText);
        ///     if (result == null)
        ///     {
        ///         throw new Exception($"didn't find translation of '{sourceText}'.");
        ///     }
        /// 
        ///     return result;
        /// }
        /// </code>
        /// 
        /// But since TrueMyth wants to help you stop using exceptions for control flow, you might rewrite it like this:
        /// <code>
        /// Maybe&lt;string&gt; GetTranslation(string sourceText)
        /// {
        ///     string result = null;
        ///     // lookup translation
        ///     result = translationService.Lookup(sourceText);
        /// 
        ///     return Maybe&lt;string&gt;.Of(result);
        /// }
        /// </code>
        /// 
        /// However, this is somewhat verbose.  It's annoying to have to specify to the type system what you mean when it 
        /// should already know. Instead, using the type system and this implicit operator, you can write the following.
        /// 
        /// <code>
        /// Maybe&lt;string&gt; GetTranslation(string sourceText)
        /// {
        ///     string result = null;
        ///     // lookup translation
        ///     result = translationService.Lookup(sourceText);
        /// 
        ///     return result;
        /// }
        /// </code>
        /// </example>
        public static implicit operator Maybe<TValue>(TValue value) => Maybe<TValue>.Of(value);

        #endregion
    }
}
