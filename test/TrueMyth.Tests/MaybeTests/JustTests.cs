using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TrueMyth.Tests.MaybeTests
{
    [TestFixture]
    public class JustTests
    {
        public Func<string, int> lengthFn => s => s.Length;

        [Test]
        public void Constructor()
        {
            var theJust = new Just<object[]>(new object[] {});
            Assert.IsInstanceOf<Just<object[]>>(theJust);
        }

        [Test]
        public void IsJust()
        {
            var theJust = new Just<object[]>(new object[] { });
            Assert.IsTrue(theJust.IsJust);
        }

        [Test]
        public void IsNothing()
        {
            var theJust = new Just<object[]>(new object[] { });
            Assert.IsFalse(theJust.IsNothing);
        }

        [Test]
        public void Map()
        {
            Func<int, int> plus2 = x => x + 2;
            var theValue = 12;
            var theJust = new Just<int>(theValue);
            var theResult = new Just<int>(plus2(theValue));

            Assert.AreEqual(theResult, theJust.Map(plus2));
        }

        [Test]
        public void MapOr()
        {
            var theValue = 42;
            var theJust = new Just<int>(42);
            var theDefault = 1;
            Func<int, int> doubleFn = n => n * 2;

            Assert.AreEqual(doubleFn(theValue), theJust.MapOr(theDefault, doubleFn));
        }

        [Test]
        public void MapOrElse()
        {
            var theValue = "this is a string";
            var theJust = new Just<string>(theValue);
            Func<int> aDefault = () => 0;

            Assert.AreEqual(lengthFn(theValue), theJust.MapOrElse(aDefault, lengthFn));
        }

        [Test]
        public void Match_Test()
        {
            var theValue = "this is a string";
            var theJust = new Just<string>(theValue);

            var actual = theJust.Match(
                new Matcher<string, string>(
                Just: val => val + ", yo",
                Nothing: () => "rats, nothing"));
            Assert.AreEqual("this is a string, yo", actual);
        }

        [Test]
        public void Or()
        {

            var theJust = new Just<Dictionary<string, string>>(new Dictionary<string, string> { { "neat", "thing" } });
            var anotherJust = new Just<Dictionary<string, string>>(new Dictionary<string, string> { { "neat", "waffles" } });
            var aNothing = new Nothing<Dictionary<string, string>>();

            Assert.AreEqual(theJust, theJust.Or(anotherJust));
            Assert.AreEqual(theJust, theJust.Or(aNothing));
        }

        [Test]
        public void OrElse()
        {
            var theJust = new Just<int>(12);
            Func<IMaybe<int>> getAnotherJust = () => Maybe.Just(42);

            Assert.AreEqual(theJust, theJust.OrElse(getAnotherJust));
        }

        [Test]
        public void And()
        {
            var theJust = new Just<object>(new { neat = "thing" });
            var theConsequentJust = new Just<object[]>(new object[] { "amazing", new { tuple = "thing" } });
            var aNothing = new Nothing<object>();

            Assert.AreEqual(theConsequentJust, theJust.And(theConsequentJust));
            Assert.AreEqual(aNothing, theJust.And(aNothing));
        }

        [Test]
        // AndThen, Chain, FlatMap
        public void AndThen()
        {
            var theValue = new Dictionary<string, string> { { "Jedi", "Luke Skywalker" } };
            var theJust = new Just<Dictionary<string, string>>(theValue);
            Func<Dictionary<string, string>, Just<string>> toDescription = 
                dict 
                => new Just<string>(string.Join("\n", dict.Keys.Select(key => $"{dict[key]} is a {key}")));

            var theExpectedResult = toDescription(theValue);
            Assert.AreEqual(theExpectedResult, theJust.AndThen(toDescription));
    }

        [Test]
        public void Unwrap()
        {
            var theValue = "value";
            var theJust = new Just<string>(theValue);

            Assert.AreEqual(theValue, theJust.UnsafelyUnwrap());
            Assert.DoesNotThrow(() => theJust.UnsafelyUnwrap());
        }

        [Test]
        public void UnwrapOr()
        {
            var theValue = new[] { 1, 2, 3 };
            var theJust = new Just<int[]>(theValue);
            int[] theDefaultValue = new int[] {};

            Assert.AreEqual(theValue, theJust.UnwrapOr(theDefaultValue));
        }

        [Test]
        public void UnwrapOrElse()
        {
            var value = "value";
            var theJust = new Just<string>(value);

            Assert.AreEqual(value, theJust.UnwrapOrElse(() => "other value"));
        }

        [Test]
        public void ToOkOrErr()
        {
            var value = "string";
            var theJust = new Just<string>(value);
            var errValue = new Dictionary<string, string> { { "reason", "such badness" } };

            Assert.AreEqual(new Ok<string, Dictionary<string, string>>(value), theJust.ToOkOrErr(errValue));
    }

        [Test]
        public void ToOkOrElseErr()
        {
            var value = new [] { "neat" };
            var theJust = new Just<string[]>(value);
            var errValue = 24;
            Func<int> getErrValue = () => errValue;

            Assert.AreEqual(new Ok<string[], int>(value), theJust.ToOkOrElseErr(getErrValue));
        }

        [Test]
        public void ToString_Test()
        {
            Assert.AreEqual("Just(42)", Maybe.Of(42).ToString());
        }

        [Test]
        public void Equals()
        {
            var a = new Just<string>("a");
            var b = new Just<string>("a");
            var c = new Just<string>("b");
            var d = Maybe.Nothing<string>();

            Assert.IsTrue(a.Equals(b));
            Assert.IsFalse(b.Equals(c));
            Assert.IsFalse(c.Equals(d));
        }

        [Test]
        public void Apply()
        {
            Func<int, string> toString = a => a.ToString();
            var fn = new Just<Func<int, string>>(toString);
            var val = new Just<int>(3);
            var result = fn.Apply(val);

            Assert.IsTrue(result.Equals(Maybe.Of("3")));
        }
    }
}
