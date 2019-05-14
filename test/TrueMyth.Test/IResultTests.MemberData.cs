using System;
using System.Collections.Generic;

namespace TrueMyth.Test
{
    using TrueMyth.Algebraic;
    using Maybe = TrueMyth.Algebraic.Maybe;
    using Result = TrueMyth.Algebraic.Result;

    public partial class IResultTests
    {
        public static IEnumerable<object[]> GetAndTheoryData()
        {
            yield return new object[] {Result.Ok<int,string>(7), Result.Ok<double,string>(8.0), Result.Ok<double,string>(8.0)};
            yield return new object[] {Result.Err<int,string>("error"), Result.Err<double,string>("error"), Result.Err<double,string>("error")};
        }

        public static IEnumerable<object[]> GetMapTheoryData()
        {
            yield return new object[] {Result.Ok<int,string>(7), 8.0, Result.Ok<double,string>(8.0)};
            yield return new object[] {Result.Err<int,string>("error"), 9.0, Result.Err<double,string>("error")};
        }

        public static IEnumerable<object[]> GetMapReturnTheoryData()
        {
            // result, mapped value, default value, expectation
            yield return new object[] {Result.Ok<int,string>(7), 8.0, 1.0, 8.0};
            yield return new object[] {Result.Err<int,string>("error"), 9.0, 1.0, 1.0};
        }

        public static IEnumerable<object[]> GetMapErrTheoryData()
        {
            yield return new object[] {Result.Err<int,string>("test"), -1, Result.Err<int,int>(-1)};
            yield return new object[] {Result.Ok<int, string>(7), -1, Result.Ok<int,int>(7)};
        }

        public static IEnumerable<object[]> GetMatchTheoryData()
        {
            yield return new object[] {Result.Ok<int,string>(7), 1.0, -1.0, 1.0};
            yield return new object[] {Result.Err<int,string>("test"), 1.0, -1.0, -1.0};
        }

        public static IEnumerable<object[]> GetOrTheoryData()
        {
            // result, default, expectation
            yield return new object[] {Result.Ok<int,string>(7), Result.Ok<int,string>(8), Result.Ok<int,string>(7)};
            yield return new object[] {Result.Err<int,string>("test"), Result.Ok<int,string>(1), Result.Ok<int,string>(1)};
        }

        public static IEnumerable<object[]> GetToMaybeTheoryData()
        {
            // result, default, expectation
            yield return new object[] {Result.Ok<int,string>(7), Maybe.Of(7)};
            yield return new object[] {Result.Err<int,string>("test"), Maybe.Nothing<int>()};
        }

        public static IEnumerable<object[]> GetUnwrapTheoryData()
        {
            yield return new object[] {Result.Ok<int,string>(7), -1, 7};
            yield return new object[] {Result.Err<int,string>("test"), -1, -1};
        }

        public static IEnumerable<object[]> GetEqualPairs()
        {
            yield return new[] {Result.Ok<int,string>(7), Result.Ok<int,string>(7)};
            yield return new[] {Result.Err<int,string>("error"), Result.Err<int,string>("error")};
        }

        public static IEnumerable<object[]> GetUnequalPairs()
        {
            yield return new[] {Result.Ok<int,string>(7), Result.Ok<int,string>(8)};
            yield return new[] {Result.Ok<int,string>(1), Result.Err<int,string>("error")};
            yield return new[] {Result.Err<int,string>("err1"), Result.Ok<int,string>(2)};
            yield return new[] {Result.Err<int,string>("err1"), Result.Err<int,string>("err2")};
        }

        public static IEnumerable<object[]> GetToStringTests()
        {
            yield return new object[] { Result.Ok<int,string>(7), $"Ok<{7.GetType().Name}>[7]"};
            yield return new object[] { Result.Err<int,string>("error"), $"Err<{"error".GetType().Name}>[error]"};
        }

        public static IEnumerable<object[]> GetResultTheoryData1()
        {
            yield return new object[] { Result.Ok<int,string>(7) };
            yield return new object[] { Result.Err<int,string>("test") };
        }
    }
}