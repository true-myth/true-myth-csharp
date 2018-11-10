using System;
using Xunit;
using TrueMyth;

namespace TrueMyth.Test
{
    public class ResultTests
    {
        [Fact]
        public void Implicit_BasicValue_OK()
        {
            // arrange

            // act
            var result = Result<int, string>.Ok(7);

            // assert
            Assert.True(result.IsOk);
        }
    }
}
