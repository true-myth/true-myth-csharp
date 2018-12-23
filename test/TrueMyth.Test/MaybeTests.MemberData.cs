using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using TrueMyth;

namespace TrueMyth.Test
{
    public partial class MaybeTests
    {
        public static IEnumerable<object[]> GetEqualPairs()
        {
            yield return new[] {Maybe<int>.Nothing, Maybe<int>.Nothing};
            yield return new[] {Maybe.Of(7), Maybe.Of(7)};
        }

        public static IEnumerable<object[]> GetUnequalPairs()
        {
            yield return new[] {Maybe<int>.Nothing, Maybe.Of(7)};
            yield return new[] {Maybe.Of(7), Maybe.Of(8)};
        }

        public static IEnumerable<object[]> GetAndTheoryData()
        {
            yield return new object[] {Maybe.Of(7), Maybe.Of("string"), Maybe.Of("string")};
            yield return new object[] {Maybe<int>.Nothing, Maybe.Of("string"), Maybe<string>.Nothing};
        }

        public static IEnumerable<object[]> GetEqualityTheoryData1Param()
        {
            var list = GetEqualityTheoryData().ToList();
            var arrayList = list.Select(arr => arr.Take(1).ToArray());
            return arrayList;
        }

        public static IEnumerable<object[]> GetEqualityTheoryData2Params()
        {
            var list = GetEqualityTheoryData().ToList();
            return list.Select(arr => arr.Take(2).ToArray());
        }

        public static IEnumerable<object[]> GetEqualityTheoryData()
        {
            yield return new object[] {Maybe.Of(7), Maybe.Of(7), true};
            yield return new object[] {Maybe.Of(8), Maybe.Of(7), false};
            yield return new object[] {Maybe<int>.Nothing, Maybe.Of(7), false};
            yield return new object[] {Maybe<int>.Nothing, Maybe<int>.Nothing, true};
        }

        public static IEnumerable<object[]> GetEqualityTheoryReferenceData1Param()
        {
            var list = GetEqualityTheoryReferenceData().ToList();
            return list.Select(arr => arr.Take(1).ToArray());
        }

        public static IEnumerable<object[]> GetEqualityTheoryReferenceData2Params()
        {
            var list = GetEqualityTheoryReferenceData().ToList();
            return list.Select(arr => arr.Take(2).ToArray());
        }

        public static IEnumerable<object[]> GetEqualityTheoryReferenceData()
        {
            yield return new object[] {Maybe.Of("test"), Maybe.Of("test"), true};
            yield return new object[] {Maybe.Of("test"), Maybe<string>.Nothing, false};
            yield return new object[] {Maybe.Of("test"), Maybe.Of("test2"), false};
            yield return new object[] {Maybe<string>.Nothing, Maybe<string>.Nothing, true};
        }

        public static IEnumerable<object[]> GetTransitiveTheoryData()
        {
            yield return new object[] {Maybe.Of(7), Maybe.Of(7), Maybe.Of(7)};
            yield return new object[] {Maybe<int>.Nothing, Maybe<int>.Nothing, Maybe<int>.Nothing};
        }

        public static IEnumerable<object[]> GetMapTheoryData()
        {
            yield return new object[] { Maybe.Of(7), new Func<int, string>((int i) => i.ToString()), Maybe.Of("7") };
            yield return new object[] { Maybe<int>.Nothing, new Func<int, string>((int i) => i.ToString()), Maybe<string>.Nothing };
        }

        public static IEnumerable<object[]> GetMapReturnTheoryData()
        {
            yield return new object[] { Maybe.Of(7), new Func<int, string>((int i) => i.ToString()), "default", "7"};
            yield return new object[] { Maybe<int>.Nothing, new Func<int, string>((int i) => i.ToString()), "default", "default"};
        }

        // input maybe, or maybe, expectation
        public static IEnumerable<object[]> GetOrTheoryData()
        {
            yield return new object[] { Maybe.Of(7), Maybe.Of(8), Maybe.Of(7) };
            yield return new object[] { Maybe<int>.Nothing, Maybe.Of(8), Maybe.Of(8) };
        }

        public static IEnumerable<object[]> GetToResultTheoryData()
        {
            yield return new object[] { Maybe.Of(7), (string)null, Result<int,string>.Ok(7) };
            yield return new object[] { Maybe<int>.Nothing, "nothing", Result<int,string>.Err("nothing") };
        }

        public static IEnumerable<object[]> GetToStringTheoryData()
        {
            yield return new object[] { Maybe.Of(7), $"Just<{typeof(int)}>[7]" };
            yield return new object[] { Maybe<int>.Nothing, $"Nothing<{typeof(int)}>" };
            yield return new object[] { Maybe.Of("test"), $"Just<{typeof(string)}>[test]" };
            yield return new object[] { Maybe<string>.Nothing, $"Nothing<{typeof(string)}>" };
        }

        public static IEnumerable<object[]> GetUnsafelyUnwrapTheoryValueTypeData()
        {
            yield return new object[] { Maybe.Of(7), new int?(7) };
            yield return new object[] { Maybe<int>.Nothing, new int?() };
        }

        public static IEnumerable<object[]> GetUnwrapTheoryData()
        {
            yield return new object[] { Maybe.Of(7), 0, 7 };
            yield return new object[] { Maybe<int>.Nothing, 0, 0 };
        }
    }
}