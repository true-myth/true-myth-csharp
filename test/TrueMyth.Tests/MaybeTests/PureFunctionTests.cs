using System;
using NUnit.Framework;

namespace TrueMyth.Tests.MaybeTests
{
    [TestFixture]
    public class PureFunctionTests
    {
        public Func<string, int> GetLength => s => s.Length;
        
        class Neat
        {
            public string neat { get; set; }
        }

        [Test]
        public void Just_Test()
        {
            var theJust = TrueMyth.Maybe.Just("string");
            Assert.IsTrue(theJust is Just<string>);
            Assert.AreEqual("string", theJust.UnsafelyUnwrap());
            Assert.Throws<ArgumentNullException>(() => TrueMyth.Maybe.Just<object>(null));
        }

        [Test]
        public void Nothing_Test()
        {
            var theNothing = TrueMyth.Maybe.Nothing<string>();
            Assert.IsInstanceOf<IMaybe<string>>(theNothing);
        }

        [Test]
        public void Map_Test()
        {
            var justAString = TrueMyth.Maybe.Just("string");
            var itsLength = TrueMyth.Maybe.Map(GetLength, justAString);
            Assert.IsInstanceOf<IMaybe<int>>(itsLength);
            Assert.AreEqual(TrueMyth.Maybe.Just("string".Length), itsLength);

            var none = TrueMyth.Maybe.Nothing<string>();
            var noLength = TrueMyth.Maybe.Map(GetLength, none);
            Assert.IsInstanceOf<IMaybe<string>>(none);
            Assert.AreEqual(TrueMyth.Maybe.Nothing<int>(), noLength);
        }

        [Test]
        public void MapOr_Test()
        {
            var justAString = TrueMyth.Maybe.Of("string");

            Assert.AreEqual("string".Length, TrueMyth.Maybe.MapOr(0, GetLength, justAString));
            Assert.AreEqual(0, TrueMyth.Maybe.MapOr(0, GetLength, TrueMyth.Maybe.Of<string>(null)));

            Assert.AreEqual(TrueMyth.Maybe.MapOr(0, GetLength, justAString), TrueMyth.Maybe.MapOr<string, int>(0)(GetLength)(justAString));
            Assert.AreEqual(TrueMyth.Maybe.MapOr(0, GetLength, justAString), TrueMyth.Maybe.MapOr(0, GetLength)(justAString));
        }

        [Test]
        public void MapOrElse_Test()
        {
            var theValue = "a string";
            var theDefault = 0;
            Func<int> toDefault = () => theDefault;
            var aJust = TrueMyth.Maybe.Just(theValue);
            var aNothing = TrueMyth.Maybe.Nothing<string>();

            Assert.AreEqual(theValue.Length, TrueMyth.Maybe.MapOrElse(toDefault, GetLength, aJust));
            Assert.AreEqual(theDefault, TrueMyth.Maybe.MapOrElse(toDefault, GetLength, aNothing));

            Assert.AreEqual(TrueMyth.Maybe.MapOrElse(toDefault, GetLength, aJust), TrueMyth.Maybe.MapOrElse<string, int>(toDefault)(GetLength)(aJust));
            Assert.AreEqual(TrueMyth.Maybe.MapOrElse(toDefault, GetLength, aJust), TrueMyth.Maybe.MapOrElse(toDefault, GetLength)(aJust));
        }

        [Test]
        public void Match_Test()
        {
            var theValue = "a string";
            var aJust = TrueMyth.Maybe.Just(theValue);
            var aNothing = TrueMyth.Maybe.Nothing<string>();

            var matcher = new Matcher<string, string>(
                Just: val => val + ", yo",
                Nothing: () => "rats, nothing"
                );

            Assert.AreEqual("a string, yo", TrueMyth.Maybe.Match(matcher, aJust));
            Assert.AreEqual("rats, nothing", TrueMyth.Maybe.Match(matcher, aNothing));

            Assert.AreEqual(TrueMyth.Maybe.Match(matcher, aJust), TrueMyth.Maybe.Match(matcher)(aJust));
        }

        [Test]
        public void And_Test()
        {
            var aJust = TrueMyth.Maybe.Just(42);
            var anotherJust = TrueMyth.Maybe.Just("a string");
            var aNothing = TrueMyth.Maybe.Nothing<string>();

            Assert.AreEqual(anotherJust, TrueMyth.Maybe.And(anotherJust, aJust));

            Assert.AreEqual(aNothing, TrueMyth.Maybe.And(aNothing, aJust));
            Assert.AreEqual(aNothing, TrueMyth.Maybe.And(aNothing, aNothing));

            Assert.AreEqual(TrueMyth.Maybe.And(aNothing, aJust), TrueMyth.Maybe.And<int, string>(aNothing)(aJust));
        }

        [Test]
        public void AndThen_Test()
        {
            Func<string, IMaybe<int>> toMaybeNumber = x => TrueMyth.Maybe.Just(int.Parse(x));
            Func<string, IMaybe<int>> toNothing = _ => TrueMyth.Maybe.Nothing<int>();

            var theValue = "42";
            var theJust = TrueMyth.Maybe.Just(theValue);
            var theExpectedResult = toMaybeNumber(theValue);
            var noString = TrueMyth.Maybe.Nothing<string>();
            var noNumber = TrueMyth.Maybe.Nothing<int>();

            Assert.AreEqual(theExpectedResult, TrueMyth.Maybe.AndThen(toMaybeNumber, theJust));
            Assert.AreEqual(noNumber, TrueMyth.Maybe.AndThen(toNothing, theJust));
            Assert.AreEqual(noNumber, TrueMyth.Maybe.AndThen(toMaybeNumber, noString));
            Assert.AreEqual(noNumber, TrueMyth.Maybe.AndThen(toNothing, theJust));
            Assert.AreEqual(TrueMyth.Maybe.AndThen(toMaybeNumber, theJust), TrueMyth.Maybe.AndThen(toMaybeNumber)(theJust));

            Assert.AreEqual(theExpectedResult, TrueMyth.Maybe.Chain(toMaybeNumber, theJust));
            Assert.AreEqual(noNumber, TrueMyth.Maybe.Chain(toNothing, theJust));
            Assert.AreEqual(noNumber, TrueMyth.Maybe.Chain(toMaybeNumber, noString));
            Assert.AreEqual(noNumber, TrueMyth.Maybe.Chain(toNothing, theJust));
            Assert.AreEqual(TrueMyth.Maybe.Chain(toMaybeNumber, theJust), TrueMyth.Maybe.Chain(toMaybeNumber)(theJust));

            Assert.AreEqual(theExpectedResult, TrueMyth.Maybe.FlatMap(toMaybeNumber, theJust));
            Assert.AreEqual(noNumber, TrueMyth.Maybe.FlatMap(toNothing, theJust));
            Assert.AreEqual(noNumber, TrueMyth.Maybe.FlatMap(toMaybeNumber, noString));
            Assert.AreEqual(noNumber, TrueMyth.Maybe.FlatMap(toNothing, theJust));
            Assert.AreEqual(TrueMyth.Maybe.FlatMap(toMaybeNumber, theJust), TrueMyth.Maybe.FlatMap(toMaybeNumber)(theJust));
        }

        [Test]
        public void Or()
        {
            var justAnswer = TrueMyth.Maybe.Of("42");
            var justWaffles = TrueMyth.Maybe.Of("waffles");
            var nothing = TrueMyth.Maybe.Nothing<string>();

            Assert.AreEqual(justWaffles, TrueMyth.Maybe.Or(justAnswer, justWaffles));
            Assert.AreEqual(justWaffles, TrueMyth.Maybe.Or(nothing, justWaffles));
            Assert.AreEqual(justAnswer, TrueMyth.Maybe.Or(justAnswer, nothing));
            Assert.AreEqual(nothing, TrueMyth.Maybe.Or(nothing, nothing));

            Assert.AreEqual(TrueMyth.Maybe.Or(justAnswer, justWaffles), TrueMyth.Maybe.Or(justAnswer)(justWaffles));
        }

        [Test]
        public void OrElse_Test()
        {
            Func<IMaybe<string>> getWaffles = () => TrueMyth.Maybe.Of("waffles");
            var just42 = TrueMyth.Maybe.Of("42");

            Assert.AreEqual(TrueMyth.Maybe.Just("42"), TrueMyth.Maybe.OrElse(getWaffles, just42));
            Assert.AreEqual(TrueMyth.Maybe.Just("waffles"), TrueMyth.Maybe.OrElse(getWaffles, TrueMyth.Maybe.Of<string>(null)));
            Assert.AreEqual(TrueMyth.Maybe.Just("42"), TrueMyth.Maybe.OrElse(() => TrueMyth.Maybe.Of<string>(null), just42));
            Assert.AreEqual(TrueMyth.Maybe.Nothing<string>(), TrueMyth.Maybe.OrElse(() => TrueMyth.Maybe.Of<string>(null), TrueMyth.Maybe.Of<string>(null)));

            Assert.AreEqual(TrueMyth.Maybe.OrElse(getWaffles, just42), TrueMyth.Maybe.OrElse(getWaffles)(just42));
        }

        [Test]
        public void Unwrap_Test()
        {
            Assert.AreEqual("42", TrueMyth.Maybe.UnsafelyUnwrap(TrueMyth.Maybe.Of("42")));
            Assert.Throws<Exception>(() => TrueMyth.Maybe.UnsafelyUnwrap(TrueMyth.Maybe.Nothing<string>()));
        }

        [Test]
        public void UnwrapOr_Test()
        {
            var theValue = new[] { 1, 2, 3 };
            var theDefaultValue = new int[] { };

            var theJust = TrueMyth.Maybe.Of(theValue);
            var theNothing = TrueMyth.Maybe.Nothing<int[]>();

            Assert.AreEqual(theValue, TrueMyth.Maybe.UnwrapOr(theDefaultValue, theJust));
            Assert.AreEqual(theDefaultValue, TrueMyth.Maybe.UnwrapOr(theDefaultValue, theNothing));
            Assert.AreEqual(TrueMyth.Maybe.UnwrapOr(theDefaultValue, theJust), TrueMyth.Maybe.UnwrapOr(theDefaultValue)(theJust));
        }

        [Test]
        public void UnwrapOrElse_Test()
        {
            var val = 100;
            Func<int> getVal = () => val;
            var just42 = TrueMyth.Maybe.Of(42);

            Assert.AreEqual(42, TrueMyth.Maybe.UnwrapOrElse(getVal, just42));
            Assert.AreEqual(val, TrueMyth.Maybe.UnwrapOrElse(getVal, TrueMyth.Maybe.Nothing<int>()));
            Assert.AreEqual(TrueMyth.Maybe.UnwrapOrElse(getVal, just42), TrueMyth.Maybe.UnwrapOrElse(getVal)(just42));
        }

        [Test]
        public void ToOkOrErr_Test()
        {
            var theValue = "string";
            var theJust = TrueMyth.Maybe.Of(theValue);
            object errValue = new { reason = "such badness" };

            Assert.AreEqual(new Ok<string, object>(theValue), TrueMyth.Maybe.ToOkOrErr(errValue, theJust));
            Assert.AreEqual(new Err<string, object>(errValue), TrueMyth.Maybe.ToOkOrErr(errValue, TrueMyth.Maybe.Nothing<string>()));
            Assert.AreEqual(TrueMyth.Maybe.ToOkOrErr(errValue, theJust), TrueMyth.Maybe.ToOkOrErr<string, object>(errValue)(theJust));
        }

        [Test]
        public void ToOkOrElseErr_Test()
        {
            var theJust = TrueMyth.Maybe.Of(12);
            var errValue = 24;
            Func<int> getErrValue = () => errValue;

            Assert.AreEqual(new Ok<int, int>(12), TrueMyth.Maybe.ToOkOrElseErr(getErrValue, theJust));
            Assert.AreEqual(new Err<int, int>(errValue), TrueMyth.Maybe.ToOkOrElseErr(getErrValue, TrueMyth.Maybe.Nothing<int>()));
            Assert.AreEqual(TrueMyth.Maybe.ToOkOrElseErr(getErrValue, theJust), TrueMyth.Maybe.ToOkOrElseErr<int, int>(getErrValue)(theJust));
        }

        [Test]
        public void FromResult_Test()
        {
            var value = 1000;
            var anOk = new Ok<int, int>(value);

            Assert.AreEqual(TrueMyth.Maybe.Just(value), TrueMyth.Maybe.FromResult(anOk));

            var reason = "oh teh noes";
            var anErr = new Err<string, string>(reason);

            Assert.AreEqual(TrueMyth.Maybe.Nothing<string>(), TrueMyth.Maybe.FromResult(anErr));
        }

        [Test]
        public void ToString_Test()
        {
            Assert.AreEqual("Just(42)", TrueMyth.Maybe.ToString(TrueMyth.Maybe.Of(42)));
            Assert.AreEqual("Nothing", TrueMyth.Maybe.ToString(TrueMyth.Maybe.Nothing<string>()));
        }

        [Test]
        public void Equals_Test()
        {
            var a = TrueMyth.Maybe.Of("a");
            var b = TrueMyth.Maybe.Of("a");
            var c = TrueMyth.Maybe.Nothing<string>();
            var d = TrueMyth.Maybe.Nothing<string>();

            Assert.IsTrue(TrueMyth.Maybe.Equals(b, a));
            Assert.IsTrue(TrueMyth.Maybe.Equals(b)(a));
            Assert.IsTrue(TrueMyth.Maybe.Equals(d, c));
            Assert.IsTrue(TrueMyth.Maybe.Equals(d)(c));
            Assert.IsFalse(TrueMyth.Maybe.Equals(c, b));
            Assert.IsFalse(TrueMyth.Maybe.Equals(c)(b));
        }

        [Test]
        public void Apply_Test()
        {
            Func<int, Func<int, int>> add = a => b => a + b;
            var maybeAdd = TrueMyth.Maybe.Of(add);

            Assert.AreEqual(TrueMyth.Maybe.Of(6), maybeAdd.Apply(TrueMyth.Maybe.Of(1)).Apply(TrueMyth.Maybe.Of(5)));

            var maybeAdd3 = TrueMyth.Maybe.Of(add(3));
            var val = TrueMyth.Maybe.Of(2);

            Assert.AreEqual(TrueMyth.Maybe.Just(5), TrueMyth.Maybe.Apply(maybeAdd3, val));
        }



        [TestFixture]
        public class OfTests
        {
            [Test]
            public void WithNullTest()
            {
                var nothingFromNull = TrueMyth.Maybe.Of<string>(null);
                Assert.IsInstanceOf<IMaybe<string>>(nothingFromNull);
                Assert.IsFalse(TrueMyth.Maybe.IsJust(nothingFromNull));
                Assert.IsTrue(TrueMyth.Maybe.IsNothing(nothingFromNull));
                Assert.Throws<Exception>(() => TrueMyth.Maybe.UnsafelyUnwrap(nothingFromNull));
            }

            [Test]
            public void WithValuesTest()
            {
                var aJust = TrueMyth.Maybe.Of<Neat>(new Neat { neat = "strings" });
                Assert.IsInstanceOf<IMaybe<Neat>>(aJust);
                var aNothing = TrueMyth.Maybe.Of<Neat>(null);
                Assert.IsInstanceOf<IMaybe<Neat>>(aNothing);

                var justANumber = TrueMyth.Maybe.Of(42);
                Assert.IsInstanceOf<IMaybe<int>>(justANumber);
                Assert.IsTrue(TrueMyth.Maybe.IsJust(justANumber));
                Assert.IsFalse(TrueMyth.Maybe.IsNothing(justANumber));
                Assert.AreEqual(42, TrueMyth.Maybe.UnsafelyUnwrap(justANumber));
            }
        }
    }
}
