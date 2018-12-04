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

### Maybe Mapping
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

### Result Mapping
|                                     TypeScript                                     |                                  C♯                                   |
| ---------------------------------------------------------------------------------- | --------------------------------------------------------------------- |
| [`and<T,U,E>(andResult: Result<U,E>, result: Result<T,E>): Result<U,E>`]()         | [`Result<U,E> Result<T,E>.And<U>(Result<UValue,TError> andResult)`]() |
| [`andThen<T,U,E>(thenFn: function, result: Result<T,E>): Result<U,E>`]()           | [`Result<U,E> Result<T,E>.AndThen<U>(Func<TValue, UValue> bindFn)`]() |
| [`ap<T,U,E>(resultFn: Result<function, E>, result: Result<T,E>): Result<U,E>]()    | No equivalent.                                                        |
| [`equals<T,E>(resultB: Result<T,E>, resultA: Result<T,E>): boolean`]()             | [`bool Object.Equals(object)`]()                                      |
| [`err<T,E>(error: E): Result<T, E>`]()                                             | [`Result<T,E> Result<T,E>.Err(E error)`]()                            |
| [`err<T,E>(): Result<T,E>]()                                                       | No equivalent.                                                        |
| [`fromMaybe<T,E>(errValue: E, maybe: Maybe<T>): Result<T,E>`]()                    | [`Result<T,E> Result.From<T,E>(Maybe<T> maybe, E error)`]()           |
| [`isErr<T,E>(result: Result<T,E>): boolean`]()                                     | [`bool Result<T,E>.IsErr`]()                                          |
| [`isInstance<T,E>(item: any): boolean`]()                                          | Not applicable.                                                       |
| [`isOk<T,E>(result: Result<T,E>): boolean`]()                                      | [`bool Result<T,E>.IsOk`]()                                           |
| [`map<T,U,E>(mapFn: function, result: Result<T,E>): Result<U,E>`]()                | [`Result<U,E> Result<T,E>.Map<U>(Func<T,U> mapFn)`]()                 |
| [`mapErr<T,E,F>(mapErrFn: function, result: Result<T,E>): Result<T,F>`]()          | [`Result<T,F> Result<T,E>.MapErr<F>(Func<E,F> mappErrFn)`]()          |
| [`mapOr<T,U,E>(orU: U, mapFn: function, result: Result<T,E>): U`]()                | [`U Result<T,E>.MapReturn(Func<T,U> mapFn, U defaultValue)`]()        |
| [`mapOrElse<T,U,E>(orElseFn: function, mapFn, function, result: Result<T,E>): U]() | See `match` / `Result<T,E>.Match<U>`                                  |
| [`match<T,E,A>(matcher: Matcher<T,E,A>, result: Result<T,E>): A]()                 | [`U Result<T,E>.Match<U>(Func<T,U> okFn, Func<E,U> errFn)`]()         |
| [`ok<T,E>() : Result<Unit,E>`]()                                                   | No equivalent.                                                        |
| [`ok<T,E>(T value): Result<T,E>`]()                                                | [`Result<T,E>.Ok(T value)`]()                                         |
| [`or<T,E,F>(defaultResult: Result<T,F>, result: Result<T,E>): Result<T,F>`]()      | [`Result<T,F> Result<T,E>.Or<F>(Result<T,F> defaultResult)`]()        |
| [`orElse<T,E,F>(elseFn: function, result: Result<T,E>): Result<T,F>`]()            | [`Result<T,F> Result<T,E>.OrElse(Func<Result<T,F>> elseFn)`]()        |
| [`toMaybe<T>(result: Result<T, any>): Maybe<T>`]()                                 | [`Maybe<T> Result<T,E>.ToMaybe()`]()                                  |
| [`toString<T,E>(result: Result<T,E>): string`]()                                   | [`string Object.ToString()`]()                                        |
| [`tryOr<T,E>(error: E, callback: function): Result<T,E>`]()                        | [`Result<T,E> Result.Try<T,E>(Func<T> tryFn, E error)`]()             |
| [`tryOrElse<T,E>(onError: function, callback: function): Result<T,E>]()            | [`Result<T,E> Result.Try(Func<T> tryFn, Func<E> errFn)`]()            |
| [`unsafelyUnwrap<T,E>(result: Result<T,E>): T`]()                                  | [`T Result<T,E>.UnsafelyUnwrap()`]()                                  |
| [`unsafelyUnwrapErr<T,E>(result: Result<T,E>): E`]()                               | [`E Result<T,E>.UnsafelyUnwrapErr()`]()                               |
| [`unwrapOr<T,E>(defaultValue: T, result: Result<T,E>): T`]()                       | [`T Result<T,E>.Unwrap(T defaultValue)`]()                            |
| [`unwrapOrElse<T,E>(orElseFn: function, result: Result<T,E>)`]()                   | [`T Result<T,E>.Unwrap(Func<E,T> errFn)`]()                           |
