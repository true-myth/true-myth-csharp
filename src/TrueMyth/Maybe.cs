using System;
using System.Collections.Generic;
using System.Linq;

namespace TrueMyth
{
    public static class Maybe
    {
        public static Maybe<T> Just<T>(T value) => Maybe<T>.Just(value);
        public static Maybe<T> From<T>(Nullable<T> nullable) where T : struct => nullable.HasValue ? Maybe<T>.Just(nullable.Value) : Maybe<T>.Nothing;
        public static Maybe<T> From<T,TError>(Result<T, TError> result) => result.UnwrapOrElse(e => Maybe<T>.Nothing);
        // #### static methods

        // All
        public static Maybe<IEnumerable<T>> MaybeAll<T>(this IEnumerable<Maybe<T>> maybes) => maybes.All(m => m.IsJust)
            ? Maybe<IEnumerable<T>>.Just(maybes.Select(m => (T)m))
            : Maybe<IEnumerable<T>>.Nothing;
        
        // Find
        public static Maybe<IEnumerable<T>> MaybeFind<T>(this IEnumerable<T> list, Func<T,bool> predicate)
        {
            var result = list?.Where(t => predicate(t));
            if (result == null || !result.Any())
            {
                return Maybe<IEnumerable<T>>.Nothing;
            }
            else
            {
                return Maybe<IEnumerable<T>>.Just(result);
            }
        }

        // Get
        public static Maybe<TValue> MaybeGet<TKey, TValue>(this ICollection<KeyValuePair<TKey, TValue>> collection, TKey key) => 
            collection.Any(kvp => EqualityComparer<TKey>.Default.Equals(kvp.Key, key)) 
            ? Maybe<TValue>.Just(collection.First(kvp => EqualityComparer<TKey>.Default.Equals(kvp.Key, key)).Value)
            : Maybe<TValue>.Nothing;
        
        // Head
        public static Maybe<TValue> MaybeFirst<TValue>(this IEnumerable<TValue> list) => list.Any() ? Maybe<TValue>.Just(list.First()) : Maybe<TValue>.Nothing;
        
        // Last
        public static Maybe<TValue> MaybeLast<TValue>(this IEnumerable<TValue> list) => list.Any() ? Maybe<TValue>.Just(list.Last()) : Maybe<TValue>.Nothing;
        
        // Tuple
        public static Maybe<(T, U)> MaybeAll<T,U>(this Tuple<Maybe<T>,Maybe<U>> tuple) => tuple.Item1.IsJust && tuple.Item2.IsJust 
            ? Maybe<(T,U)>.Just(((T)tuple.Item1, (U)tuple.Item2)) 
            : Maybe<(T,U)>.Nothing;

        public static Maybe<(T,U,V)> MaybeAll<T,U,V>(this Tuple<Maybe<T>,Maybe<U>,Maybe<V>> tuple) => tuple.Item1.IsJust && tuple.Item2.IsJust && tuple.Item3.IsJust
            ? Maybe<(T,U,V)>.Just(((T)tuple.Item1, (U)tuple.Item2, (V)tuple.Item3))
            : Maybe<(T,U,V)>.Nothing;

        public static Maybe<(T,U,V,W)> MaybeAll<T,U,V,W>(this Tuple<Maybe<T>,Maybe<U>,Maybe<V>,Maybe<W>> tuple) => tuple.Item1.IsJust && tuple.Item2.IsJust && tuple.Item3.IsJust && tuple.Item4.IsJust
            ? Maybe<(T,U,V,W)>.Just(((T)tuple.Item1, (U)tuple.Item2, (V)tuple.Item3, (W)tuple.Item4))
            : Maybe<(T,U,V,W)>.Nothing;
    }

    public sealed class Maybe<TValue>
    {
        private readonly TValue _value;
        private readonly bool _isJust;

        public bool IsJust => _isJust;
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

        public static Maybe<TValue> Just(TValue value) => new Maybe<TValue>(value);
        public static Maybe<TValue> Nothing { get; } = new Maybe<TValue>();

        // #### instance methods
        public Maybe<UValue> And<UValue>(Maybe<UValue> andMaybe) => this._isJust ? andMaybe : Maybe<UValue>.Nothing;
        
        // AndThen
        public Maybe<UValue> And<UValue>(Func<TValue,Maybe<UValue>> thenFn) => this._isJust ? thenFn(this._value) : Maybe<UValue>.Nothing;
        
        // Apply?
        
        // Map
        public Maybe<UValue> Map<UValue>(Func<TValue,UValue> mapFn) => this._isJust ? Maybe<UValue>.Just(mapFn(this._value)) : Maybe<UValue>.Nothing;
        
        // MapOr
        public Maybe<UValue> Map<UValue>(Func<TValue,UValue> mapFn, UValue defaultValue) => this._isJust ? Maybe<UValue>.Just(mapFn(this._value)) : Maybe<UValue>.Just(defaultValue);

        // MapOrElse
        public Maybe<UValue> Map<UValue>(Func<TValue,UValue> mapFn, Func<UValue> elseFn) => this._isJust ? Maybe<UValue>.Just(mapFn(this._value)) : Maybe<UValue>.Just(elseFn());
        
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
            if (this._isJust)
            {
                return (this._value?.GetHashCode() ?? 0) | typeof(TValue).GetHashCode() | 0xbeef;
            }
            else
            {
                return typeof(TValue).GetHashCode() | 0xdead;
            }
        }

        // #### implicits (instead of .of)
        public static implicit operator TValue(Maybe<TValue> maybe) => maybe.UnsafelyUnwrap();
        public static implicit operator Maybe<TValue>(TValue value) => new Maybe<TValue>(value);
        // TODO: need an implicit for Nothing
    }
}