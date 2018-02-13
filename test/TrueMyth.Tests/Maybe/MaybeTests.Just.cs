using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrueMyth.Tests
{
    public partial class MaybeTests
    {
        [TestFixture]
        public class Just_
        {
            [Test]
            public void Constructor()
            {
            }

            [Test]
            public void IsJust()
            {
            }

            [Test]
            public void IsNothing()
            {
            }

            [Test]
            public void Map()
            {
            }

            [Test]
            public void MapOr()
            {
            }

            [Test]
            public void MapOrElse()
            {
            }

            [Test]
            public void Match_Test()
            {
                var theValue = "this is a string";
                var theJust = Maybe.Just(theValue);

                var actual = theJust.Match(
                    Just: val => val + ", yo",
                    Nothing: () => "rats, nothing");
                Assert.AreEqual("this is a string, yo", actual);
            }

            [Test]
            public void And()
            {
            }

            [Test]
            // AndThen, Chain, FlatMap
            public void AndThen()
            {
            }

            [Test]
            public void Or()
            {
            }

            [Test]
            public void OrElse()
            {
            }

            [Test]
            public void Unwrap()
            {
            }

            [Test]
            public void UnwrapOr()
            {
            }

            [Test]
            public void UnwrapOrElse()
            {
            }

            [Test]
            public void ToOkOrErr()
            {
            }

            [Test]
            public void ToOkOrElseErr()
            {
            }

            [Test]
            public void FromResult()
            {
            }

            [Test]
            public void ToString()
            {
            }

            [Test]
            public void Equals()
            {
            }

            [Test]
            public void Apply()
            {
            }
        }
    }
}
