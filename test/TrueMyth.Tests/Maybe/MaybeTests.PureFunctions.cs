using NUnit.Framework;
using System;

namespace TrueMyth.Tests
{
    public partial class MaybeTests
    {
        [TestFixture]
        public class PureFunctions
        {
            public Func<string, int> GetLength => s => s.Length;

            [Test]
            public void Just_Test()
            {
                var theJust = Maybe.Just("string");
                Assert.IsTrue(theJust is Maybe<string>);
                Assert.AreEqual("string", theJust.UnsafelyUnwrap());
                Assert.Throws<ArgumentNullException>(() => Maybe.Just<object>(null));
            }

            [Test]
            public void Nothing_Test()
            {
                var theNothing = Maybe.Nothing;
                Assert.IsTrue(theNothing is Maybe);
            }

            [Test]
            public void Map_Test()
            {
                var justAString = Maybe.Just("string");
                var itsLength = justAString.Map(GetLength);
                Assert.IsTrue(itsLength is Maybe<int>);
                Assert.AreEqual(Maybe.Just("string".Length), itsLength);

                var none = Maybe.Nothing;
                var noLength = none.Map(GetLength);
                Assert.IsTrue(noLength is Maybe<int>);
                Assert.IsTrue(noLength == Maybe.Nothing);
                Assert.AreEqual(Maybe.Nothing, noLength);
            }

            [Test]
            public void MapOr_Test()
            {
                var theValue = 42;
                var theJust = Maybe.Just(42);
                var theDefault = 1;
                Func<int, int> doubleFunc = n => n * 2;

                Assert.AreEqual(doubleFunc(theValue), theJust.MapOr(theDefault, doubleFunc));
            }

            [Test]
            public void MapOrElse_Test()
            {
                var theValue = "this is a string";
                var theJust = Maybe.Just(theValue);
                Func<int> aDefault = () => 0;

                Assert.AreEqual(GetLength(theValue), theJust.MapOrElse(aDefault, GetLength));
            }

            [Test]
            public void Match_Test()
            {
                var theValue = "a string";
                var aJust = Maybe.Just(theValue);
                var aNothing = Maybe.Nothing;

                var matcher = new Matcher<string, string>(
                    Just: val => val + ", yo",
                    Nothing: () => "rats, nothings"
                    );

                var actual = aJust.Match(matcher);
                Assert.AreEqual("a string, yo", actual);

                expect(Maybe.match(matcher, aJust)).toEqual("a string, yo");
                expect(Maybe.match(matcher, aNothing)).toEqual("rats, nothing");

                expect(Maybe.match(matcher)(aJust)).toEqual(Maybe.match(matcher, aJust));
        }

            [Test]
            public void And_Test()
            {
                var theJust = Maybe.Just(new { neat = "thing" });
                var theConsequentJust = Maybe.Just(new { a = new object[] { "amazing", new { tuple = "thing" } } });
                var aNothing = Maybe.Nothing;

                Assert.AreEqual(theConsequentJust, theJust.And(theConsequentJust));
                //Assert.AreEqual(aNothing, theJust.And(aNothing));
    }

            [Test]
            // AndThen, Chain, FlatMap
            public void AndThen_Test()
            {
                //var theValue = new { Jedi = "Luke Skywalker" };
                //var theJust = Maybe.Just(theValue);
                //var toDescription = (dict: { [key: string]: string}) =>
                //Maybe.Just(
                //    Object.keys(dict)
                //        .map(key => `${ dict[key]} is a ${key}`).join("\n"));

                //const theExpectedResult = toDescription(theValue);
                //expect(theJust[method](toDescription)).toEqual(theExpectedResult);
            }

            [Test]
            public void Or()
            {
                var theJust = Maybe.Just(new { neat = "thing" });
                var anotherJust = Maybe.Just(new { neat = "waffles" });
                var aNothing = Maybe.Nothing;

                Assert.AreEqual(theJust, theJust.Or(anotherJust));
                Assert.AreEqual(theJust, theJust.Or(aNothing));
            }

            [Test]
            public void OrElse_Test()
            {
                var theJust = Maybe.Just(12);
                Func<Maybe<int>> getAnotherJust = () => Maybe.Just(42);
               
                Assert.AreEqual(theJust, theJust.OrElse(getAnotherJust));
            }

            [Test]
            public void Unwrap_Test()
            {
            }

            [Test]
            public void UnwrapOr_Test()
            {
            }

            [Test]
            public void UnwrapOrElse_Test()
            {
            }

            [Test]
            public void ToOkOrErr_Test()
            {
            }

            [Test]
            public void ToOkOrElseErr_Test()
            {
            }

            [Test]
            public void FromResult_Test()
            {
            }

            [Test]
            public void ToString_Test()
            {
                Assert.AreEqual("Just(42)", Maybe.Of(42).ToString());
                Assert.AreEqual("Nothing", Maybe.Nothing.ToString());
            }

            [Test]
            public void Equals_Test()
            {
                var a = Maybe.Of("a");
                var b = Maybe.Of("a");
                var c = Maybe.Nothing;
                var d = Maybe.Nothing;

                Assert.IsTrue(b.Equals(a));
                Assert.IsFalse(c.Equals(b));
                Assert.IsTrue(d.Equals(c));
            }

            [Test]
            public void Apply_Test()
            {
                Func<int, Func<int, int>> add = (a) => (b) => a + b;
                var maybeAdd = Maybe.Of(add);

                Assert.AreEqual(Maybe.Of(6), maybeAdd.Apply(Maybe.Of(1)).Apply(Maybe.Of(5)));

                var maybeAdd3 = Maybe.Of(add(3));
                var val = Maybe.Of(2);

                Assert.AreEqual(Maybe.Just(5), maybeAdd3.Apply(val));
            }
        }
    }
}
