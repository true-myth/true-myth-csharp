using NUnit.Framework;
using System;

namespace TrueMyth.Tests.MaybeTests
{
    [TestFixture]
    public class IntegrationTests
    {
        public Func<string, int> lengthFn => s => s.Length;

        [Test]
        public void Interacting()
        {
            var aMaybe = Maybe.Nothing<string>();
            var mapped = aMaybe.Map(lengthFn);
            var anotherMaybe = Maybe.Just(10);
            var anotherMapped = anotherMaybe.MapOr("nada", n => $"The number was { n}");

            Assert.IsInstanceOf<Nothing<int>>(mapped);
            Assert.IsNotInstanceOf<Just<int>>(mapped);
            Assert.AreEqual("The number was 10", anotherMapped);
        }
    }
}
