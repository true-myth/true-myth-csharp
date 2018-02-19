using NUnit.Framework;
using System;

namespace TrueMyth.Tests
{
    [TestFixture]
    public class PureFunctionTests
    {
        public Func<string, int> GetLength => s => s.Length;

        [Test]
        public void Just_Test()
        {
            var theJust = Maybe.Just("string");
            Assert.IsTrue(theJust is Just<string>);
            Assert.AreEqual("string", theJust.UnsafelyUnwrap());
            Assert.Throws<ArgumentNullException>(() => Maybe.Just<object>(null));
        }

        [Test]
        public void Nothing_Test()
        {
            var theNothing = Maybe.Nothing<string>();
            Assert.IsInstanceOf<IMaybe<string>>(theNothing);
        }

        [Test]
        public void Map_Test()
        {
            var justAString = Maybe.Just("string");
            var itsLength = Maybe.Map(GetLength, justAString);
            Assert.IsInstanceOf<IMaybe<int>>(itsLength);
            Assert.AreEqual(Maybe.Just("string".Length), itsLength);

            var none = Maybe.Nothing<string>();
            var noLength = Maybe.Map(GetLength, none);
            Assert.IsInstanceOf<IMaybe<string>>(none);
            Assert.AreEqual(Maybe.Nothing<int>(), noLength);
        }

        [Test]
        public void MapOr_Test()
        {
            var justAString = Maybe.Of("string");

            Assert.AreEqual("string".Length, Maybe.MapOr(0, GetLength, justAString));
            Assert.AreEqual(0, Maybe.MapOr(0, GetLength, Maybe.Of<string>(null)));

            Assert.AreEqual(Maybe.MapOr(0, GetLength, justAString), Maybe.MapOr<string, int>(0)(GetLength)(justAString));
            Assert.AreEqual(Maybe.MapOr(0, GetLength, justAString), Maybe.MapOr(0, GetLength)(justAString));
        }

        [Test]
        public void MapOrElse_Test()
        {
            var theValue = "a string";
            var theDefault = 0;
            Func<int> toDefault = () => theDefault;
            var aJust = Maybe.Just(theValue);
            var aNothing = Maybe.Nothing<string>();

            Assert.AreEqual(theValue.Length, Maybe.MapOrElse(toDefault, GetLength, aJust));
            Assert.AreEqual(theDefault, Maybe.MapOrElse(toDefault, GetLength, aNothing));

            Assert.AreEqual(Maybe.MapOrElse(toDefault, GetLength, aJust), Maybe.MapOrElse<string, int>(toDefault)(GetLength)(aJust));
            Assert.AreEqual(Maybe.MapOrElse(toDefault, GetLength, aJust), Maybe.MapOrElse(toDefault, GetLength)(aJust));
        }

        [Test]
        public void Match_Test()
        {
            var theValue = "a string";
            var aJust = Maybe.Just(theValue);
            var aNothing = Maybe.Nothing<string>();

            var matcher = new Matcher<string, string>(
                Just: val => val + ", yo",
                Nothing: () => "rats, nothing"
                );

            Assert.AreEqual("a string, yo", Maybe.Match(matcher, aJust));
            Assert.AreEqual("rats, nothing", Maybe.Match(matcher, aNothing));

            Assert.AreEqual(Maybe.Match(matcher, aJust), Maybe.Match(matcher)(aJust));
        }

        [Test]
        public void And_Test()
        {
            var aJust = Maybe.Just(42);
            var anotherJust = Maybe.Just("a string");
            var aNothing = Maybe.Nothing<string>();

            Assert.AreEqual(anotherJust, Maybe.And(anotherJust, aJust));

            Assert.AreEqual(aNothing, Maybe.And(aNothing, aJust));
            Assert.AreEqual(aNothing, Maybe.And(aNothing, aNothing));

            Assert.AreEqual(Maybe.And(aNothing, aJust), Maybe.And<int, string>(aNothing)(aJust));
        }

        [Test]
        public void AndThen_Test()
        {
            Func<string, IMaybe<int>> toMaybeNumber = x => Maybe.Just(int.Parse(x));
            Func<string, IMaybe<int>> toNothing = _ => Maybe.Nothing<int>();

            var theValue = "42";
            var theJust = Maybe.Just(theValue);
            var theExpectedResult = toMaybeNumber(theValue);
            var noString = Maybe.Nothing<string>();
            var noNumber = Maybe.Nothing<int>();

            Assert.AreEqual(theExpectedResult, Maybe.AndThen(toMaybeNumber, theJust));
            Assert.AreEqual(noNumber, Maybe.AndThen(toNothing, theJust));
            Assert.AreEqual(noNumber, Maybe.AndThen(toMaybeNumber, noString));
            Assert.AreEqual(noNumber, Maybe.AndThen(toNothing, theJust));
            Assert.AreEqual(Maybe.AndThen(toMaybeNumber, theJust), Maybe.AndThen(toMaybeNumber)(theJust));

            Assert.AreEqual(theExpectedResult, Maybe.Chain(toMaybeNumber, theJust));
            Assert.AreEqual(noNumber, Maybe.Chain(toNothing, theJust));
            Assert.AreEqual(noNumber, Maybe.Chain(toMaybeNumber, noString));
            Assert.AreEqual(noNumber, Maybe.Chain(toNothing, theJust));
            Assert.AreEqual(Maybe.Chain(toMaybeNumber, theJust), Maybe.Chain(toMaybeNumber)(theJust));

            Assert.AreEqual(theExpectedResult, Maybe.FlatMap(toMaybeNumber, theJust));
            Assert.AreEqual(noNumber, Maybe.FlatMap(toNothing, theJust));
            Assert.AreEqual(noNumber, Maybe.FlatMap(toMaybeNumber, noString));
            Assert.AreEqual(noNumber, Maybe.FlatMap(toNothing, theJust));
            Assert.AreEqual(Maybe.FlatMap(toMaybeNumber, theJust), Maybe.FlatMap(toMaybeNumber)(theJust));
        }

        [Test]
        public void Or()
        {
            var justAnswer = Maybe.Of("42");
            var justWaffles = Maybe.Of("waffles");
            var nothing = Maybe.Nothing<string>();

            Assert.AreEqual(justWaffles, Maybe.Or(justAnswer, justWaffles));
            Assert.AreEqual(justWaffles, Maybe.Or(nothing, justWaffles));
            Assert.AreEqual(justAnswer, Maybe.Or(justAnswer, nothing));
            Assert.AreEqual(nothing, Maybe.Or(nothing, nothing));

            Assert.AreEqual(Maybe.Or(justAnswer, justWaffles), Maybe.Or(justAnswer)(justWaffles));
        }

        [Test]
        public void OrElse_Test()
        {
            Func<IMaybe<string>> getWaffles = () => Maybe.Of("waffles");
            var just42 = Maybe.Of("42");

            Assert.AreEqual(Maybe.Just("42"), Maybe.OrElse(getWaffles, just42));
            Assert.AreEqual(Maybe.Just("waffles"), Maybe.OrElse(getWaffles, Maybe.Of<string>(null)));
            Assert.AreEqual(Maybe.Just("42"), Maybe.OrElse(() => Maybe.Of<string>(null), just42));
            Assert.AreEqual(Maybe.Nothing<string>(), Maybe.OrElse(() => Maybe.Of<string>(null), Maybe.Of<string>(null)));

            Assert.AreEqual(Maybe.OrElse(getWaffles, just42), Maybe.OrElse(getWaffles)(just42));
        }

        [Test]
        public void Unwrap_Test()
        {
            Assert.AreEqual("42", Maybe.UnsafelyUnwrap(Maybe.Of("42")));
            Assert.Throws<Exception>(() => Maybe.UnsafelyUnwrap(Maybe.Nothing<string>()));
        }

        [Test]
        public void UnwrapOr_Test()
        {
            var theValue = new[] { 1, 2, 3 };
            var theDefaultValue = new int[] { };

            var theJust = Maybe.Of(theValue);
            var theNothing = Maybe.Nothing<int[]>();

            Assert.AreEqual(theValue, Maybe.UnwrapOr(theDefaultValue, theJust));
            Assert.AreEqual(theDefaultValue, Maybe.UnwrapOr(theDefaultValue, theNothing));
            Assert.AreEqual(Maybe.UnwrapOr(theDefaultValue, theJust), Maybe.UnwrapOr(theDefaultValue)(theJust));
        }

        [Test]
        public void UnwrapOrElse_Test()
        {
            var val = 100;
            Func<int> getVal = () => val;
            var just42 = Maybe.Of(42);

            Assert.AreEqual(42, Maybe.UnwrapOrElse(getVal, just42));
            Assert.AreEqual(val, Maybe.UnwrapOrElse(getVal, Maybe.Nothing<int>()));
            Assert.AreEqual(Maybe.UnwrapOrElse(getVal, just42), Maybe.UnwrapOrElse(getVal)(just42));
        }

        [Test]
        public void ToOkOrErr_Test()
        {
            var theValue = "string";
            var theJust = Maybe.Of(theValue);
            object errValue = new { reason = "such badness" };

            Assert.AreEqual(new Ok<string, object>(theValue), Maybe.ToOkOrErr(errValue, theJust));
            Assert.AreEqual(new Err<string, object>(errValue), Maybe.ToOkOrErr(errValue, Maybe.Nothing<string>()));
            Assert.AreEqual(Maybe.ToOkOrErr(errValue, theJust), Maybe.ToOkOrErr<string, object>(errValue)(theJust));
        }

        [Test]
        public void ToOkOrElseErr_Test()
        {
            var theJust = Maybe.Of(12);
            var errValue = 24;
            Func<int> getErrValue = () => errValue;

            Assert.AreEqual(new Ok<int, int>(12), Maybe.ToOkOrElseErr(getErrValue, theJust));
            Assert.AreEqual(new Err<int, int>(errValue), Maybe.ToOkOrElseErr(getErrValue, Maybe.Nothing<int>()));
            Assert.AreEqual(Maybe.ToOkOrElseErr(getErrValue, theJust), Maybe.ToOkOrElseErr<int, int>(getErrValue)(theJust));
        }

        [Test]
        public void FromResult_Test()
        {
            var value = 1000;
            var anOk = new Ok<int, int>(value);

            Assert.AreEqual(Maybe.Just(value), Maybe.FromResult(anOk));

            var reason = "oh teh noes";
            var anErr = new Err<string, string>(reason);

            Assert.AreEqual(Maybe.Nothing<string>(), Maybe.FromResult(anErr));
        }

        [Test]
        public void ToString_Test()
        {
            Assert.AreEqual("Just(42)", Maybe.ToString(Maybe.Of(42)));
            Assert.AreEqual("Nothing", Maybe.ToString(Maybe.Nothing<string>()));
        }

        [Test]
        public void Equals_Test()
        {
            var a = Maybe.Of("a");
            var b = Maybe.Of("a");
            var c = Maybe.Nothing<string>();
            var d = Maybe.Nothing<string>();

            Assert.IsTrue(Maybe.Equals(b, a));
            Assert.IsTrue(Maybe.Equals(b)(a));
            Assert.IsTrue(Maybe.Equals(d, c));
            Assert.IsTrue(Maybe.Equals(d)(c));
            Assert.IsFalse(Maybe.Equals(c, b));
            Assert.IsFalse(Maybe.Equals(c)(b));
        }

        [Test]
        public void Apply_Test()
        {
            Func<int, Func<int, int>> add = a => b => a + b;
            var maybeAdd = Maybe.Of(add);

            Assert.AreEqual(Maybe.Of(6), maybeAdd.Apply(Maybe.Of(1)).Apply(Maybe.Of(5)));

            var maybeAdd3 = Maybe.Of(add(3));
            var val = Maybe.Of(2);

            Assert.AreEqual(Maybe.Just(5), Maybe.Apply(maybeAdd3, val));
        }
    }
}
