using System;
using Xunit;
using TrueMyth;
using System.Collections.Generic;

namespace TrueMyth.Test
{
    using SimpleResult = Result<int, string>;

    public partial class ResultTests
    {
        public static IEnumerable<object[]> GetEqualPairs()
        {
            yield return new[] {SimpleResult.Ok(7), SimpleResult.Ok(7)};
            yield return new[] {SimpleResult.Err("error"), SimpleResult.Err("error")};
        }

        public static IEnumerable<object[]> GetUnequalPairs()
        {
            yield return new[] {SimpleResult.Ok(7), SimpleResult.Ok(8)};
            yield return new[] {SimpleResult.Ok(1), SimpleResult.Err("error")};
            yield return new[] {SimpleResult.Err("err1"), SimpleResult.Ok(2)};
            yield return new[] {SimpleResult.Err("err1"), SimpleResult.Err("err2")};
        }

        public static IEnumerable<object[]> GetToStringTests()
        {
            yield return new object[] { SimpleResult.Ok(7), "Ok[7]"};
            yield return new object[] { SimpleResult.Err("error"), "Err[error]"};
        }
    }
}