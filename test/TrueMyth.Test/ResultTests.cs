using System;
using Xunit;
using TrueMyth;

namespace TrueMyth.Test
{
    using SimpleResult = Result<int, string>;

    public partial class ResultTests
    {
        [Fact]
        public void Implicit_Ok_OK()
        {
            // act
            var result = SimpleResult.Ok(7);

            // assert
            Assert.True(result.IsOk);
            Assert.False(result.IsErr);
        }

        [Fact]
        public void Implicit_Error_OK()
        {
            // act
            var result = SimpleResult.Err("message");

            // assert
            Assert.True(result.IsErr);
            Assert.False(result.IsOk);
        }

        [Fact]
        public void Implict_Value_Ok()
        {
            // act
            var result = SimpleResult.Ok(7);

            // assert
            Assert.True(result.IsOk);
            Assert.False(result.IsErr);
            Assert.Equal<int>(7, result);
        }

        [Fact]
        public void Implicit_Error_Ok()
        {
            // act
            var result = SimpleResult.Err("message");

            // assert
            Assert.True(result.IsErr);
            Assert.False(result.IsOk);
            Assert.Equal("message", result);
        }

        [Fact]
        public void Explicit_NullError_Ok()
        {
            // act
            var result = SimpleResult.Err(null);

            // assert
            Assert.True(result.IsErr);
            Assert.False(result.IsOk);
            Assert.Null(result.UnsafelyUnwrapErr());
        }

        [Fact]
        public void Implicit_ReturnOk_Ok()
        {
            SimpleResult f() => 7;

            // act
            var result = f();

            // assert
            Assert.True(result.IsOk);
            Assert.False(result.IsErr);
            Assert.Equal<int>(7, result);
        }

        [Fact]
        public void Implicit_ReturnErr_Ok()
        {
            SimpleResult f() => "something bad";

            // act
            var result = f();

            // assert
            Assert.True(result.IsErr);
            Assert.False(result.IsOk);
            Assert.Equal("something bad", result);
        }

        [Fact]
        public void UnwrapError_AsValue_Throws()
        {
            // arrange
            var result = SimpleResult.Err("something bad happened");

            // act
            // assert
            Assert.True(result.IsErr);
            Assert.False(result.IsOk);
            Assert.Throws<InvalidOperationException>(() => result.UnsafelyUnwrap());
        }

        [Fact]
        public void UnwrapOk_AsError_Throw()
        {
            var result = SimpleResult.Ok(8);

            // act
            // assert
            Assert.True(result.IsOk);
            Assert.False(result.IsErr);
            Assert.Throws<InvalidOperationException>(() => result.UnsafelyUnwrapErr());
        }

        [Fact]
        public void And_OkReturnsAndResult_Ok()
        {
            // arrange
            var result = SimpleResult.Ok(7);
            var andResult = SimpleResult.Ok(1);

            // act
            var r = result.And(andResult);

            // assert
            Assert.Equal(andResult, r);
        }

        [Fact]
        public void And_ErrReturnsThis_Ok()
        {
            // arrange
            var result = SimpleResult.Err("error");
            var andResult = SimpleResult.Ok(0);

            // act
            var r = result.And(andResult);

            // assert
            Assert.Equal(result, r);
        }

        // Match
        [Fact]
        public void Match_OkCallsOk_Ok()
        {
            // arrange
            var result = SimpleResult.Ok(7);

            // act
            var functionCalled = result.Match(
                ok: value => "ok",
                err: error => "error"
            );

            // Assert
            Assert.Equal("ok", functionCalled);
        }

        // Or
        [Fact]
        public void Or_OkReturnsThis_Ok()
        {
            // arrange
            var r1 = SimpleResult.Ok(0);
            var r2 = SimpleResult.Ok(1);

            // act
            var r = r1.Or(r2);

            // assert
            Assert.Equal(r1, r);
        }

        [Fact]
        public void Or_ErrReturnsThat_Ok()
        {
            // arrange
            var r1 = SimpleResult.Err("error");
            var r2 = SimpleResult.Ok(0);
            
            // act
            var r = r1.Or(r2);

            // assert
            Assert.Equal(r2, r);
        }

        // OrElse
        [Fact]
        public void OrElse_Ok()
        {
            // arrange
            var r1 = SimpleResult.Ok(0);
            var r2 = SimpleResult.Ok(1);

            // act
            var r = r1.OrElse(() => r2);

            // assert
            Assert.Equal<SimpleResult>(r1, r);
        }

        [Fact]
        public void OrElse_Err()
        {
            // arrange
            var r1 = SimpleResult.Err("error");
            var r2 = SimpleResult.Ok(0);

            // act
            var r = r1.OrElse(() => r2);

            // assert
            Assert.Equal<SimpleResult>(r2, r);
        }

        // Select
        [Fact]
        public void Select_Ok()
        {
            // arrange
            var r1 = SimpleResult.Ok(0);

            // act
            var r = r1.Select(value => value.ToString());

            // assert
            Assert.NotNull(r);
            Assert.True(r.IsOk);
            Assert.Equal("0", r.UnsafelyUnwrap());
        }

        [Fact]
        public void SelectErr_Ok()
        {
            // arrange
            var r1 = SimpleResult.Err("error");

            // act
            var r = r1.SelectErr(error => error.ToUpperInvariant());

            // assert
            Assert.NotNull(r);
            Assert.True(r.IsErr);
            Assert.Equal("ERROR", r.UnsafelyUnwrapErr());
        }

        [Fact]
        public void  SelectErrOk_Ok()
        {
            // arrange
            var r1 = SimpleResult.Ok(0);

            // act
            var r = r1.SelectErr(error => Convert.ToInt16(error));

            // assert
            Assert.NotNull(r);
            Assert.True(r.IsOk);
        }

        [Fact]
        public void AndThen_Ok()
        {
            // arrange
            var r1 = SimpleResult.Ok(0);

            // act
            var r = r1.AndThen(() => Result<float, string>.Ok(3.7f));

            // assert
            Assert.NotNull(r);
            Assert.True(r.IsOk);
            Assert.Equal(3.7f, r.UnsafelyUnwrap());
        }

        [Fact]
        public void AndThen_Err_Ok()
        {
            // arrange
            var r1 = SimpleResult.Err("error");

            // act
            var r = r1.AndThen(() => Result<float,string>.Ok(3.7f));

            // assert
            Assert.NotNull(r);
            Assert.True(r.IsErr);
            Assert.Equal("error", r.UnsafelyUnwrapErr());
        }

        [Fact]
        public void SelectOrDefaultOk_Ok()
        {
            // arrange
            var r1 = SimpleResult.Ok(0);

            // act
            var r = r1.SelectOrDefault(i => i.ToString(), "default");

            // assert
            Assert.NotNull(r);
            Assert.Equal("0", r);
        }

        [Fact]
        public void SelectOrDefault_Err_Ok()
        {
            // arrange
            var r1 = SimpleResult.Err("error");

            // act
            var r = r1.SelectOrDefault(i => i.ToString(), "default");

            // assert
            Assert.NotNull(r);
            Assert.Equal("default", r);
        }

        // SelectOrElse
        [Fact]
        public void SelectOrElse_Ok_Ok()
        {
            // arrange
            var r1 = SimpleResult.Ok(0);

            // act
            var r = r1.SelectOrElse(i => i, s => -1);

            // arrange
            Assert.Equal(0, r);
        }

        [Fact]
        public void SelectOrElse_Err_Ok()
        {
            // arrange
            var r1 = SimpleResult.Err("error");

            // act
            var r = r1.SelectOrElse(i => i, s => -1);

            // assert
            Assert.Equal(-1, r);
        }

        // ToString
        [Theory]
        [MemberData(nameof(GetToStringTests))]
        public void ToString_Ok(SimpleResult r1, string expectedString)
        {
            // arrange - in theory
            // act
            var r = r1.ToString();

            // assert
            Assert.NotNull(r);
            Assert.Equal(expectedString, r);
        }

        // UnwrapOr
        [Fact]
        public void UnwrapOr_Ok_Ok()
        {
            // arrange
            var r1 = SimpleResult.Ok(7);

            // act
            var r = r1.UnwrapOr(0);

            // assert
            Assert.Equal(7, r);
        }

        [Fact]
        public void UnwrapOr_Err_Ok()
        {
            // arrange
            var r1  = SimpleResult.Err("error");

            // act
            var r = r1.UnwrapOr(0);

            // assert
            Assert.Equal(0, r);
        }

        // UnwrapOrElse        
        [Fact]
        public void UnwrapOrElse_Ok_Ok()
        {
            // arrange
            var r1 = SimpleResult.Ok(7);

            // act
            var r = r1.UnwrapOrElse(err => 0);

            // assert
            Assert.Equal(7, r);
        }

        [Fact]
        public void UnwrapOrElse_Err_Ok()
        {
            // arrange
            var r1 = SimpleResult.Err("error");

            // act
            var r = r1.UnwrapOrElse(err => 0);

            // assert
            Assert.Equal(0, r);
        }

        [Theory]
        [MemberData(nameof(GetEqualPairs))]
        public void Equals_Same_True(SimpleResult r1, SimpleResult r2)
        {
            // arrange
            // act
            var equalityResult = r1.Equals(r2);

            // assert
            Assert.True(equalityResult);
        }

        [Theory]
        [MemberData(nameof(GetUnequalPairs))]
        public void Equals_Different_False(SimpleResult r1, SimpleResult r2)
        {
            // arrange - in theory
            // act
            var equalityResult = r1.Equals(r2);

            // assert
            Assert.False(equalityResult);
        }

        // GetHashCode
        [Theory]
        [MemberData(nameof(GetEqualPairs))]
        public void GetHashCode_SameResults_Equal(SimpleResult r1, SimpleResult r2)
        {
            // arrange - in theory
            // act
            var hc1 = r1.GetHashCode();
            var hc2 = r2.GetHashCode();

            // assert
            Assert.Equal(hc1, hc2);
        }

        [Theory]
        [MemberData(nameof(GetUnequalPairs))]
        public void GetHashCode_DifferentResult_NotEqual(SimpleResult r1, SimpleResult r2)
        {
            // arrange - in theory
            // act
            var hc1 = r1.GetHashCode();
            var hc2 = r2.GetHashCode();

            // assert
            Assert.NotEqual(hc1, hc2);
        }
    }
}
