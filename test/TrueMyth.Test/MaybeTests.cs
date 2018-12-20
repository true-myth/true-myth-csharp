using System;
using System.Collections.Generic;
using Xunit;
using TrueMyth;

namespace TrueMyth.Test
{
    public partial class MaybeTests
    {
        [Fact]
        public void MaybeAll_Ok()
        {
            // arrange
            var listOfMaybes = new [] {
                Maybe.Of(1),
                Maybe.Of(2),
                Maybe.Of(3),
            };

            // act
            var maybeList = Maybe.All(listOfMaybes);

            // assert
            Assert.True(maybeList.IsJust);
            Assert.False(maybeList.IsNothing);
            Assert.IsAssignableFrom(typeof(IEnumerable<int>), maybeList.UnsafelyUnwrap());
            Assert.Equal(new[]{1,2,3}, maybeList.UnsafelyUnwrap());
        }

        [Fact]
        public void MaybeAll_Mixed_Nothing()
        {
            // arrange
            var listOfMaybes = new[] {
                Maybe.Of(1),
                Maybe<int>.Nothing,
                Maybe.Of(3),
            };

            // act
            var maybeList = Maybe.All(listOfMaybes);

            // assert
            Assert.True(maybeList.IsNothing);
            Assert.False(maybeList.IsJust);
        }

        public void AsMaybe_Ok()
        {

        }
    }
}