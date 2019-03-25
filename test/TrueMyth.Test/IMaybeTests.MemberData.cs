using System;
using System.Collections.Generic;
using System.Linq;
using TrueMyth.Algebraic;

namespace TrueMyth.Test
{
    using Maybe = TrueMyth.Algebraic.Maybe;
    using Result = TrueMyth.Algebraic.Result;

    public partial class IMaybeTests
    {
        public static IEnumerable<object[]> GetAndTheoryData()
        {
            yield return new object[] {Maybe.Of(7), Maybe.Of("string"), Maybe.Of("string")};
            yield return new object[] {Maybe.Nothing<int>(), Maybe.Of("string"), Maybe.Nothing<string>()};
        }

        public static IEnumerable<object[]> GetAndThenTheoryData()
        {
            yield return new object[] {Maybe.Of(7), new Func<int,IMaybe<double>>(i => Maybe.Of(1.0/i)), Maybe.Of(1.0/7)};
            yield return new object[] {Maybe.Nothing<int>(), new Func<int,IMaybe<double>>(i => Maybe.Of(1.0/i)), Maybe.Nothing<double>()};
        }

        public static IEnumerable<object[]> GetMapTheoryData()
        {
            yield return new object[] { Maybe.Of(7), new Func<int, string>(i => i.ToString()), Maybe.Of("7") };
            yield return new object[] { Maybe.Nothing<int>(), new Func<int, string>(i => i.ToString()), Maybe.Nothing<string>() };
        }

        public static IEnumerable<object[]> GetMapReturnTheoryData()
        {
            yield return new object[] { Maybe.Of(7), new Func<int, string>(i => i.ToString()), "default", "7"};
            yield return new object[] { Maybe.Nothing<int>(), new Func<int, string>(i => i.ToString()), "default", "default"};
        }

        public static IEnumerable<object[]> GetEqualityTheoryData1Param() =>
            GetEqualityTheoryData().Select(arr => arr.Take(1).ToArray());

        public static IEnumerable<object[]> GetEqualityTheoryData2Params() => 
            GetEqualityTheoryData().Select(arr => arr.Take(2).ToArray());

        public static IEnumerable<object[]> GetEqualityTheoryData()
        {
            yield return new object[] {Maybe.Of(7), Maybe.Of(7), true};
            yield return new object[] {Maybe.Of(8), Maybe.Of(7), false};
            yield return new object[] {Maybe.Nothing<int>(), Maybe.Of(7), false};
            yield return new object[] {Maybe.Nothing<int>(), Maybe.Nothing<int>(), true};
        }

        public static IEnumerable<object[]> GetTransitiveEqualityTheoryData()
        {
            yield return new object[] {Maybe.Of(7), Maybe.Of(7), Maybe.Of(7)};
            yield return new object[] {Maybe.Nothing<int>(), Maybe.Nothing<int>(), Maybe.Nothing<int>()};
            yield return new object[] {Maybe.Of("test"), Maybe.Of("test"), Maybe.Of("test")};
        }

        public static IEnumerable<object[]> GetOrTheoryData()
        {
            yield return new object[] { Maybe.Of(7), Maybe.Of(8), Maybe.Of(7) };
            yield return new object[] { Maybe.Nothing<int>(), Maybe.Of(8), Maybe.Of(8) };
        }

        public static IEnumerable<object[]> GetOfTheoryData()
        {
            yield return new object[] {typeof(int), 7, new Just<int>(7)};
            yield return new object[] {typeof(string), "test", new Just<string>("test")};
            yield return new object[] {typeof(string), null, new Nothing<string>()};
            yield return new object[] {typeof(int?), (int?)7, new Just<int?>((int?)7)};
            yield return new object[] {typeof(int?), (int?)null, new Nothing<int?>()};
        }

        public static IEnumerable<object[]> GetToResultTheoryData()
        {
            yield return new object[] { Maybe.Of(7), (string)null, new Ok<int,string>(7) };
            yield return new object[] { Maybe.Nothing<int>(), "nothing", new Err<int,string>("nothing") };
        }

        public static IEnumerable<object[]> GetToStringTheoryData()
        {
            yield return new object[] { Maybe.Of(7), $"Just<{typeof(int)}>[7]" };
            yield return new object[] { Maybe.Nothing<int>(), $"Nothing<{typeof(int)}>" };
            yield return new object[] { Maybe.Of("test"), $"Just<{typeof(string)}>[test]" };
            yield return new object[] { Maybe.Nothing<string>(), $"Nothing<{typeof(string)}>" };
        }

        public static IEnumerable<object[]> GetUnsafelyUnwrapTheoryData()
        {
            yield return new object[] { typeof(int), Maybe.Of(7), true, 7 };
            yield return new object[] { typeof(int), Maybe.Nothing<int>(), false, null };
            yield return new object[] { typeof(string), Maybe.Of("test"), true, "test" };
            yield return new object[] { typeof(string), Maybe.Nothing<string>(), false, null };
        }

        public static IEnumerable<object[]> GetUnwrapTheoryData()
        {
            yield return new object[] { Maybe.Of(7), 0, 7 };
            yield return new object[] { Maybe.Nothing<int>(), 0, 0 };
        }
    }
}