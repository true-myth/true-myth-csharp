using System;
using Xunit;
using TrueMyth;

namespace TrueMyth.Test
{
    using SimpleResult = Result<int, string>;

    public class ResultTests
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
    }
}
