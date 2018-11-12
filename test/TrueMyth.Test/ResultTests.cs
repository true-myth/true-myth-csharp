using System;
using Xunit;
using TrueMyth;

namespace TrueMyth.Test
{
    public class ResultTests
    {
        [Fact]
        public void Implicit_Ok_OK()
        {
            // act
            var result = Result<int, string>.Ok(7);

            // assert
            Assert.True(result.IsOk);
        }

        [Fact]
        public void Implicit_Error_OK()
        {
            // act
            var result = Result<int, string>.Err("message");

            // assert
            Assert.True(result.IsErr);
        }

        [Fact]
        public void Implict_Value_Ok()
        {
            // act
            var result = Result<int, string>.Ok(7);

            // assert
            Assert.True(result.IsOk);
            Assert.Equal<int>(7, result);
        }

        [Fact]
        public void Implicit_Error_Ok()
        {
            // act
            var result = Result<int, string>.Err("message");

            // assert
            Assert.True(result.IsErr);
            Assert.Equal("message", result);
        }

        [Fact]
        public void Explicit_NullError_Ok()
        {
            // act
            var result = Result<int, string>.Err(null);

            // assert
            Assert.True(result.IsErr);
            Assert.Null(result.UnsafelyUnwrapErr());
        }

        [Fact]
        public void Implicit_Return_Ok()
        {
            Result<int, string> f() => 7;

            // act
            var result = f();

            // assert
            Assert.True(result.IsOk);
            Assert.Equal<int>(7, result);
        }
    }
}
