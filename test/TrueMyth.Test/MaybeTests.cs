using System;
using System.Collections.Generic;
using Xunit;
using TrueMyth;
using TrueMyth.Unsafe;

namespace TrueMyth.Test
{
    public partial class MaybeTests
    {
        [Fact]
        public void Of_ValueType_Just()
        {
            var m1 = Maybe.Of(7);

            Assert.True(m1.IsJust);
        }

        [Fact]
        public void Of_ValueType_Nothing()
        {
            var m1 = Maybe<int>.Nothing;

            Assert.True(m1.IsNothing);
        }

        [Fact]
        public void Of_ReferenceType_Just()
        {
            var m1 = Maybe.Of("string");

            Assert.True(m1.IsJust);
        }

        [Fact]
        public void Of_ReferenceType_Null()
        {
            var m1 = Maybe.Of<string>(null);

            Assert.True(m1.IsNothing);
        }

        [Theory]
        [MemberData(nameof(GetAndTheoryData))]
        public void And_Theory(Maybe<int> mInt, Maybe<string> mString, object expected)
        {
            // arrange (theory data)

            // act
            var maybeResult = mInt.And(mString);

            // assert
            Assert.Equal(expected, maybeResult);
        }

        [Fact]
        public void AndThen_Just_Ok()
        {
            // arrange
            var maybeInt = Maybe.Of(7);

            // act
            var maybeResult = maybeInt.AndThen(i => Maybe.Of(1.0/i));

            // assert
            Assert.True(maybeResult.IsJust);
            Assert.Equal(1.0/7, maybeResult.UnsafelyUnwrap());
        }

        [Fact]
        public void AndThen_Nothing_Ok()
        {
            // arrange
            var maybeInt = Maybe<int>.Nothing;

            // act
            var maybeResult = maybeInt.AndThen(i => Maybe.Of(2*i));

            // assert
            Assert.True(maybeResult.IsNothing);
            Assert.Equal(Maybe<int>.Nothing, maybeResult);
        }

        [Theory]
        [MemberData(nameof(GetEqualityTheoryData1Param))]
        [MemberData(nameof(GetEqualityTheoryReferenceData1Param))]
        public void Equals_Self_Theory(object m)
        {
            // arrange - from theory

            // act
            var result = m.Equals(m);

            // assert
            Assert.True(result);
        }

        [Theory]
        [MemberData(nameof(GetEqualityTheoryData2Params))]
        [MemberData(nameof(GetEqualityTheoryReferenceData2Params))]
        public void Equals_XY_Theory(object m1, object m2)
        {
            // arrange - theory

            // act
            var r1 = m1.Equals(m2);
            var r2 = m2.Equals(m1);

            // assert
            Assert.Equal(r1, r2);
        }

        [Theory]
        [MemberData(nameof(GetTransitiveTheoryData))]
        public void Equals_Transitive_Theory_ValueType(Maybe<int> m1, Maybe<int> m2, Maybe<int> m3)
        {
            // arrange - theory
            // act
            var r1 = m1.Equals(m2);
            var r2 = m2.Equals(m3);
            var r3 = m1.Equals(m3);

            // assert
            Assert.True(r1);
            Assert.True(r2);
            Assert.True(r3);
        }

        [Theory]
        [MemberData(nameof(GetEqualityTheoryData))]
        [MemberData(nameof(GetEqualityTheoryReferenceData))]
        public void Equals_Idempotency_Theory(object m1, object m2, bool expectation)
        {
            // arrange - theory
            // act
            for (var i = 0; i < 100; i++)
            {
                var result = m1.Equals(m2);
                Assert.Equal(expectation, result);
            }
        }

        // TODO: in RC5 need to test that x.Equals(null) == false

        [Theory]
        [MemberData(nameof(GetMapTheoryData))]
        public void Map_Theory(Maybe<int> m1, Func<int, string> mapFn, object expectation)
        {
            var result = m1.Map(mapFn);

            Assert.Equal(expectation, (object)result);
        }

        [Theory]
        [MemberData(nameof(GetMapReturnTheoryData))]
        public void MapReturn_Theory(Maybe<int> m1, Func<int,string> mapFn, string @default, string expectation)
        {
            // arrange - theory
            // act
            var result = m1.MapReturn(mapFn, @default);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [MemberData(nameof(GetEqualityTheoryData1Param))]
        public void Match_Theory(Maybe<int> m1)
        {
            // arrnage - theory
            // act
            var result = m1.Match(
                j => 
                {
                    Assert.True(m1.IsJust);
                    return 1;
                }, 
                () => 
                {
                    Assert.True(m1.IsNothing);
                    return 2;
                }
            );

            if (m1.IsJust) Assert.Equal(1, result);
            if (m1.IsNothing) Assert.Equal(2, result);
        }

        [Theory]
        [MemberData(nameof(GetEqualityTheoryData1Param))]
        public void Match_Void_Theory(Maybe<int> m1)
        {
            m1.Match(
                just: _ => {
                    Assert.True(m1.IsJust);
                },
                nothing: () => {
                    Assert.True(m1.IsNothing);
                }
            );
        }

        [Theory]
        [MemberData(nameof(GetOrTheoryData))]
        public void Or_Theory(Maybe<int> m1, Maybe<int> m2, Maybe<int> expectation)
        {
            // arrange - theory
            // act
            var result = m1.Or(m2);

            // assert
            Assert.Equal(expectation, result);
        }

        [Theory]
        [MemberData(nameof(GetOrTheoryData))]
        public void OrElse_Theory(Maybe<int> m1, Maybe<int> m2, Maybe<int> expectation)
        {
            // arrange - theory
            // act
            var result = m1.OrElse(() => m2);

            // assert
            Assert.Equal(expectation, result);
        }

        [Fact]
        public void Of_ValueType_Ok()
        {
            // arrange - n/a
            // act
            var m = Maybe.Of(7);

            Assert.True(m.IsJust);
            Assert.Equal(7, (int)m);
        }

        [Fact]
        public void Of_ReferenceType_Ok()
        {
            // arrange - n/a
            // act
            var m = Maybe.Of("test");

            Assert.True(m.IsJust);
            Assert.Equal("test", (string)m);
        }

        [Fact]
        public void Of_ReferenceType_Null_Ok()
        {
            // arrange n/a
            // act
            var m = Maybe<string>.Of(null);

            Assert.True(m.IsNothing);
        }

        [Theory]
        [MemberData(nameof(GetMapTheoryData))]
        public void Select_Theory(Maybe<int> m1, Func<int, string> mapFn, object expectation)
        {
            var result = m1.Select(mapFn);

            Assert.Equal(expectation, result);
        }

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
            //Assert.IsAssignableFrom(typeof(IEnumerable<int>), maybeList.UnsafelyUnwrap());
            Assert.Equal(new[]{1,2,3}, maybeList.UnsafelyUnwrap());
        }

        [Theory]
        [MemberData(nameof(GetToResultTheoryData))]
        public void ToResult_Theory(Maybe<int> m1, string errString, Result<int,string> expectation)
        {
            // arrange - theory
            var result = m1.ToResult(errString);

            // assert
            Assert.Equal(expectation, result);
        }

        [Theory]
        [MemberData(nameof(GetToResultTheoryData))]
        public void ToResultFunc_Theory(Maybe<int> m1, string errString, Result<int, string> expectation)
        {
            // arrange - theory
            // act
            var result = m1.ToResult(() => errString);

            // assert
            Assert.Equal(expectation, result);
        }

        [Theory]
        [MemberData(nameof(GetToStringTheoryData))]
        public void ToString_Theory(object m1, string expectation)
        {
            // arrange - theory
            // act
            var result = m1.ToString();

            // assert
            Assert.Equal(expectation, result);
        }

        [Theory]
        [MemberData(nameof(GetUnsafelyUnwrapTheoryValueTypeData))]
        public void UnsafelyUnwrap_Theory_ValueType(Maybe<int> m1, int? expectation)
        {
            // arrange - theory
            // act
            if (expectation.HasValue)
            {
                var result = m1.UnsafelyUnwrap();
                Assert.Equal(expectation.Value, result);
            }
            else
            {
                Assert.Throws<InvalidOperationException>(() => m1.UnsafelyUnwrap());
            }
        }

        [Theory]
        [MemberData(nameof(GetUnwrapTheoryData))]
        public void UnwrapOr_Theory(Maybe<int> m1, int defaultValue, int expectation)
        {
            // arrange - theory
            // act
            var result = m1.UnwrapOr(defaultValue);

            // assert
            Assert.Equal(expectation, result);
        }

        [Theory]
        [MemberData(nameof(GetUnwrapTheoryData))]
        public void UnwrapOrElse_Theory(Maybe<int> m1, int defaultValue, int expectation)
        {
            // arrange - theory
            // act
            var result = m1.UnwrapOrElse(() => defaultValue);

            // assert
            Assert.Equal(expectation, result);
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

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void IComparable_AtoA_Zero(bool useGenerics)
        {
            // arrange
            var a = Maybe<int>.Of(0);
            var a_obj = (object)a;
            
            // act
            var result = useGenerics
                ? a.CompareTo(a)
                : a.CompareTo(a_obj);

            // assert
            Assert.Equal(0, result);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void IComparable_Symmetry_Ok(bool useGenerics)
        {
            // arrange
            var a = Maybe<int>.Of(0);
            var a_obj = (object)a;
            var b = Maybe<int>.Of(0);
            var b_obj = (object)b;
            
            // act
            var abResult = useGenerics
                ? a.CompareTo(b)
                : a.CompareTo(b_obj);
            var baResult = useGenerics
                ? b.CompareTo(a)
                : b.CompareTo(a_obj);

            // assert
            Assert.Equal(0, abResult);
            Assert.Equal(0, baResult);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void IComparable_Transitive_Ok(bool useGenerics)
        {
            // arrange
            var a = Maybe<int>.Of(0);
            var a_obj = (object)a;
            var b = Maybe<int>.Of(0);
            var b_obj = (object)b;
            var c = Maybe<int>.Of(0);
            var c_obj = (object)c;
            
            // act
            var abResult = useGenerics
                ? a.CompareTo(b)
                : a.CompareTo(b_obj);
            var baResult = useGenerics
                ? b.CompareTo(a)
                : b.CompareTo(a_obj);
            var acResult = useGenerics
                ? a.CompareTo(c)
                : a.CompareTo(c_obj);

            // assert
            Assert.Equal(0, abResult);
            Assert.Equal(0, baResult);
            Assert.Equal(0, acResult);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void IComparable_Inverse_Ok(bool useGenerics)
        {
            // arrange
            var a = Maybe<int>.Of(0);
            var a_obj = (object)a;
            var b = Maybe<int>.Of(1);
            var b_obj = (object)b;

            // act
            var abResult = useGenerics
                ? a.CompareTo(b)
                : a.CompareTo(b_obj);
            var baResult = useGenerics
                ? b.CompareTo(a)
                : b.CompareTo(a_obj);

            // assert
            Assert.Equal(-1, abResult);
            Assert.Equal(1, baResult);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void IComparable_Inductive_Ok(bool useGenerics)
        {
            // arrange
            var a = Maybe<int>.Of(0);
            var a_obj = (object)a;
            var b = Maybe<int>.Of(1);
            var b_obj = (object)b;
            var c = Maybe<int>.Of(2);
            var c_obj = (object)c;

            // act
            var abResult = useGenerics
                ? a.CompareTo(b)
                : a.CompareTo(b_obj);
            var bcResult = useGenerics
                ? b.CompareTo(c)
                : b.CompareTo(c_obj);
            var acResult = useGenerics
                ? a.CompareTo(c)
                : a.CompareTo(c_obj);

            // assert
            Assert.Equal(-1, abResult);
            Assert.Equal(-1, bcResult);
            Assert.Equal(-1, acResult);
        }

        [Fact]
        public void Maybe_ToNullable_Ok()
        {
            // arrange
            int? a = 0;

            // act
            var maybe = Maybe.From(a);

            // assert
            Assert.True(maybe.IsJust);
            Assert.Equal(a.Value, (int)maybe);
        }

        [Fact]
        public void Maybe_FromNullable_Ok()
        {
            // arrange
            int? a = null;

            // act
            var maybe = Maybe.From(a);

            // assert
            Assert.True(maybe.IsNothing);
        }
    }
}