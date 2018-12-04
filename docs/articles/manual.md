# Overview

True Myth provides idiomatic, type-safe wrappers and helper functions to help
help you with two extremely common cases in programming:

 - not having a value
 - having a result where you need to deal with either success or failure

You could implement all of these yourself – it's not hard! – but it's much
easier to just have one extremely well-tested library you can use everywhere to
solve this problem once and for all.

# Comparison to other Libraries

## TrueMyth TypeScript

This TrueMyth library is derived from the original
[TrueMyth](https://github.com/true-myth/true-myth), which was designed for
TypeScript.  It was initially ported, but then later was redesigned somewhat to
be more idiomatic for C♯.  Below is a mapping of functions; you may find it
useful to be able to link back to the documentation of the original library in
cases where this documentation is lacking.

|                                      TypeScript                                       |                                                      C♯                                                       |
| ------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------- |
| [`all<T>(...maybes: T): All<T>`](https://true-myth.js.org/modules/_maybe_.html#all-1) | [`Maybe<IEnumerable<T>> Maybe.All<T>(params Maybe<T>[] maybes)`]()                                            |
| [`and<T,U>(andMaybe: Maybe<U>, maybe: Maybe<T>): Maybe<U>`]()                         | [`Maybe<U> Maybe<T>.And<U>(Maybe<U> andMaybe)`]()                                                             |
| [`andThen<T,U>(thenFn: function, maybe: Maybe<T>): Maybe<U>`]()                       | [`Maybe<U> Maybe<T>.AndThen<U>(Func<TValue, Maybe<U>> bindFn)`]()                                             |
| [`ap<T,U>(maybeFn: Maybe<function>, maybe: Maybe<T>): Maybe<U>`]()                    | No equivalent                                                                                                 |
| [`equals<T>(mb: Maybe<T>, ma: Maybe<T>): boolean`]()                                  | [`bool Object.Equals(object obj)`]()                                                                          |
| [`find<T>(predicate: Result<T,any>): Maybe<T>`]()                                     | [`Maybe<IEnumerable<T>> Maybe.MaybeFind<T>(this IEnumerable<T> list, Func<T,bool> predicate)`]()              |
| [`fromResult<T>(result: Result<T,any>): Maybe<T>`]()                                  | [`Maybe<T> Maybe.From<T,E>(Result<T,E> result)`]()                                                            |
| [`get<T,K>(key: K, maybeObj: Maybe<T>): Maybe<Required<T>[K]>`]()                     | [`Maybe<T> Maybe.MaybeGet<K,V>(this ICollection<KeyValuePair<T,K>> collection, T key)`]()                     |
| [`head<T>(array: Array<T + null + undefined>): Maybe<T>`]()                           | [`Maybe<T> Maybe.MaybeFirst<T>(this IEnumerable<T> list)`]()                                                  |
| [`isInstance<T>(item: any): boolean`]()                                               | Not applicable                                                                                                |
| [`isJust<T>(maybe: Maybe<T>)`]()                                                      | [`bool Maybe<T>.IsJust`]()                                                                                    |
| [`isNothing<T>(maybe: Maybe<T>)`]()                                                   | [`bool Maybe<T>.IsNothing`]()                                                                                 |
| [`last<T>(array: Array<T + null + undefined>): Maybe<T>`]()                           | [`Maybe<T> Maybe.MaybeLast<T>(this IEnumerable<T> list)`]()                                                   |
| [`map<T,U>(mapFn: function, maybe: Maybe<T>): Maybe<U>`]()                            | [`Maybe<U> Maybe<T>.Map<U>(Func<T,U> mapFn)`]()                                                               |
| [`mapOr<T,U>(orU: U, mapFn: function, maybe: Maybe<T>): U`]()                         | [`U Maybe<T>.MapReturn<U>(Func<T,U> mapFn, U defaultValue)`]()                                                |
| [`mapOrElse<T,U>(orElseFn: function, mapFn: function, maybe: Maybe<T>): U`]()         | See `match` / `Maybe<T>.Match<U>`                                                                             |
| [`match<T,A>(matcher: Matcher<T,A>, maybe: Maybe<T>): A`]()                           | [`U Maybe<T>.Match<U>(Func<T,U> just, Func<T> nothing)`]()                                                    |
| [`nothing<T>(_?: undefined + null): Maybe<T>`]()                                      | [`Maybe<T> Maybe<T>.Nothing`]()                                                                               |
| [`of<T>(value?: T + null): Maybe<T>`]()                                               | [`Maybe.Of(T value)`]()                                                                                       |
| [`or<T>(defaultMaybe: Maybe<T>, maybe: Maybe<T>): Maybe<T>`]()                        | [`Maybe<T> Maybe<T>.Or(Maybe<T> maybe)`]()                                                                    |
| [`orElse<T>(elseFn: function, maybe: Maybe<T>): Maybe<T>`]()                          | [`Maybe<T> Maybe<T>.OrElse(Func<Maybe<T>> elseFn)`]()                                                         |
| [`property<T,K>(key: K, obj: T):Maybe<Required<T>[K]>`]()                             | See `get` / `Maybe.MaybeGet`                                                                                  |
| [`toOkOrElse<T,E>(elseFn: function, maybe: Maybe<T>): Result<T,E>`]()                 | [`Result<T,E> Maybe<T>.ToResult<E>(Func<E> errFn)`]()                                                         |
| [`toOkOrErr<T,E>(error: E, maybe: Maybe<T>): Result<T,E>`]()                          | [`Result<T,E> Maybe<T>.ToResult<E>(E error)`]()                                                               |
| [`toString<T>(maybe: Maybe<T>): string`]()                                            | [`string Object.ToString()`]()                                                                                |
| [`tuple<T>(maybes: Maybe<T>): Maybe<[T]>`]()                                          | [`Maybe<(T,U)> Maybe.MaybeAll<T,U>(this Tuple<Maybe<T>, Maybe<U>> tuple)`]() and similarly for up to 4-tuples |
| [`unsafelyUnwrap<T>(maybe: Maybe<T>): T`]()                                           | [`T Maybe<T>.UnsafelyUnwrap()`]()                                                                             |
| [`unwrapOr<T>(defaultValue: T, maybe: Maybe<T>): T`]()                                | [`T Unwrap(T defaultValue)`]()                                                                                |
| [`unwrapOrElse<T>(orElseFn: function, maybe: Maybe<T>): T`]()                         | [`T Maybe<T>.Unwrap(Func<TValue> elseFn)`]()                                                                  |
