using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace TrueMyth.Tests
{
    [TestFixture]
    public class NothingTests
    {
        public Func<string, int> lengthFn => s => s.Length;

        class Neat
        {
            public string neat { get; set;  }
        }

        [Test]
        public void Constructor()
        {
            var theNothing = new Nothing<object>();
            Assert.IsInstanceOf<Nothing<object>>(theNothing);
        }

        [Test]
        public void IsJust()
        {
            var theNothing = new Nothing<int>();
            Assert.IsFalse(theNothing.IsJust);
        }

        [Test]
        public void IsNothing()
        {
            var theNothing = new Nothing<int>();
            Assert.IsTrue(theNothing.IsNothing);
        }

        [Test]
        public void Map()
        {
            var theNothing = new Nothing<string>();
            Assert.AreEqual(theNothing, theNothing.Map(lengthFn));
        }

        [Test]
        public void MapOr()
        {
            var theNothing = new Nothing<int>();
            var theDefaultValue = "yay";
            Assert.AreEqual(theDefaultValue, theNothing.MapOr(theDefaultValue, i => string.Empty));
        }

        [Test]
        public void MapOrElse()
        {
            var theDefaultValue = "potatoes";
            Func<string> getDefaultValue = () => theDefaultValue;
            Func<Neat, string> getNeat = x => x.neat;
            var theNothing = new Nothing<Neat>();

            Assert.AreEqual(theDefaultValue, theNothing.MapOrElse(getDefaultValue, getNeat));
        }

        [Test]
        public void Match()
        {
            var nietzsche = new Nothing<string>();
            var soDeepMan = string.Join(" ", new[]
            { "Whoever fights monsters should see to it that in the process he does not become a monster.",
                "And if you gaze long enough into an abyss, the abyss will gaze back into you."});

            var actual = nietzsche.Match(
                new Matcher<string, string>(
                    Just: s => s + ", yo",
                    Nothing: () => soDeepMan));
            Assert.AreEqual(soDeepMan, actual);
        }

        [Test]
        public void Or()
        {
            var theNothing = new Nothing<bool>();
            var theDefaultValue = Maybe.Just(false);

            Assert.AreEqual(theDefaultValue, theNothing.Or(theDefaultValue));
        }

        [Test]
        public void OrElse()
        {
            var theNothing = new Nothing<Dictionary<string, string[]>>();
            var justTheFallback = Maybe.Just(new Dictionary<string, string[]> { { "here", new [] {"to", "see"} } });
            Func<IMaybe<Dictionary<string, string[]>>> getTheFallback = () => justTheFallback;

            Assert.AreEqual(justTheFallback, theNothing.OrElse(getTheFallback));
        }

        [Test]
        public void And()
        {
            var theNothing = new Nothing<string[]>();
            var theConsequentJust = new Just<string[]>(new[] { "blaster bolts" });
            var anotherNothing = new Nothing<string[]>();

            Assert.AreEqual(theNothing, theNothing.And(theConsequentJust));
            Assert.AreEqual(theNothing, theNothing.And(anotherNothing));
        }

        [Test]
        // AndThen, Chain, FlatMap
        public void AndThen()
        {
            var theNothing = new Nothing<string>();
            var theDefaultValue = "string";
            Func<string, IMaybe<string>> getDefaultValue = s => Maybe.Just(theDefaultValue);

            Assert.AreEqual(theNothing, theNothing.AndThen(getDefaultValue));
        }

        [Test]
        public void Unwrap()
        {
            var noStuffAtAll = new Nothing<string>();
            Assert.Throws<Exception>(() => noStuffAtAll.UnsafelyUnwrap());
        }

        [Test]
        public void UnwrapOr()
        {
            var theNothing = new Nothing<int[]>();
            var theDefaultValue = new int[] { };
            Assert.AreEqual(theDefaultValue, theNothing.UnwrapOr(theDefaultValue));
        }

        [Test]
        public void UnwrapOrElse()
        {
            var theNothing = new Nothing<string>();
            var theDefaultValue = "it be all fine tho";
            Assert.AreEqual(theDefaultValue, theNothing.UnwrapOrElse(() => theDefaultValue));
        }

        [Test]
        public void ToOkOrErr()
        {
            var theNothing = new Nothing<string>();
            var errValue = new Dictionary<string, string> { { "reason", "such badness" } };

            Assert.AreEqual(new Err<string, Dictionary<string, string>>(errValue), theNothing.ToOkOrErr(errValue));
        }

        [Test]
        public void ToOkOrElseErr()
        {
            var theNothing = new Nothing<string[]>();
            var errValue = 24;
            Func<int> getErrValue = () => errValue;

            Assert.AreEqual(new Err<string[], int>(errValue), theNothing.ToOkOrElseErr(getErrValue));
        }

        [Test]
        public void ToString_Test()
        {
            Assert.AreEqual("Nothing", Maybe.Nothing<string>().ToString());
        }

        [Test]
        public void Equals()
        {
            var a = new Just<string>("a");
            var b = new Nothing<string>();
            var c = new Nothing<string>();

            Assert.IsFalse(a.Equals(b));
            Assert.IsTrue(b.Equals(c));
        }

        [Test]
        public void Apply()
        {
            var fn = new Nothing<Func<string, int>>();
            var val = new Just<string>("three");
            var result = fn.Apply(val);

            Assert.IsTrue(result.IsNothing);
        }
    }
}
