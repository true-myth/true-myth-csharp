using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;
using TrueMyth.Algebraic;
using TrueMyth.Unsafe;
using System.Reflection;

namespace TrueMyth.Test
{
    using Maybe = TrueMyth.Algebraic.Maybe;

    public partial class IMaybeTests
    {
        private readonly ITestOutputHelper _output;

        public IMaybeTests(ITestOutputHelper output)
        {
            _output = output;
        }

        #region Instance Method Tests
        

        [Theory]
        [MemberData(nameof(GetAndTheoryData))]
        public void And_Theory(IMaybe<int> mInt, IMaybe<string> mString, object expected)
        {
            // arrange
            // act
            var maybeResult = mInt.And(mString);

            // assert
            Assert.Equal(expected, maybeResult);
        }

        [Theory]
        [MemberData(nameof(GetAndThenTheoryData))]
        public void AndThen_Theory(IMaybe<int> maybe, Func<int,IMaybe<double>> thenFn, IMaybe<double> expected)
        {
            // arrange
            // act
            var maybeResult = maybe.AndThen(thenFn);

            // assert
            Assert.Equal(expected, maybeResult);
        }

        [Theory]
        [MemberData(nameof(GetMapTheoryData))]
        public void Map_Theory(IMaybe<int> m1, Func<int, string> mapFn, object expectation)
        {
            var result = m1.Map(mapFn);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [MemberData(nameof(GetMapReturnTheoryData))]
        public void MapReturn_Theory(IMaybe<int> m1, Func<int,string> mapFn, string @default, string expectation)
        {
            // arrange - theory
            // act
            var result = m1.MapReturn(mapFn, @default);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [MemberData(nameof(GetEqualityTheoryData1Param))]
        public void Match_Theory(IMaybe<int> m1)
        {
            // arrnage - theory
            // act
            var result = m1.Match(
                j => 
                {
                    Assert.True(m1 is Just<int>);
                    return 1;
                }, 
                () => 
                {
                    Assert.True(m1 is Nothing<int>);
                    return 2;
                }
            );

            if (m1 is Just<int>) Assert.Equal(1, result);
            if (m1 is Nothing<int>) Assert.Equal(2, result);
        }

        [Theory]
        [MemberData(nameof(GetEqualityTheoryData1Param))]
        public void Match_Void_Theory(IMaybe<int> m1)
        {
            m1.Match(
                just: _ => {
                    Assert.True(m1 is Just<int>);
                },
                nothing: () => {
                    Assert.True(m1 is Nothing<int>);
                }
            );
        }

        [Theory]
        [MemberData(nameof(GetOfTheoryData))]
        public void Of_Theory(Type t, object val, object expected)
        {
            // arrange
            var of_template = typeof(Maybe).GetMethod("Of");
            var of = of_template.MakeGenericMethod(t);

            // act
            var result = of.Invoke(null, new[] { val });

            // assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [MemberData(nameof(GetOrTheoryData))]
        public void Or_Theory(IMaybe<int> m1, IMaybe<int> m2, IMaybe<int> expectation)
        {
            // arrange - theory
            // act
            var result = m1.Or(m2);

            // assert
            Assert.Equal(expectation, result);
        }

        [Theory]
        [MemberData(nameof(GetOrTheoryData))]
        public void OrElse_Theory(IMaybe<int> m1, IMaybe<int> m2, IMaybe<int> expectation)
        {
            // arrange - theory
            // act
            var result = m1.OrElse(() => m2);

            // assert
            Assert.Equal(expectation, result);
        }

        [Theory]
        [MemberData(nameof(GetMapTheoryData))]
        public void Select_Theory(IMaybe<int> m1, Func<int, string> mapFn, object expectation)
        {
            var result = m1.Select(mapFn);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [MemberData(nameof(GetToResultTheoryData))]
        public void ToResult_Theory(IMaybe<int> m1, string errString, IResult<int,string> expectation)
        {
            // arrange - theory
            var result = m1.ToResult(errString);

            // assert
            Assert.Equal(expectation, result);
        }

        [Theory]
        [MemberData(nameof(GetToResultTheoryData))]
        public void ToResultFunc_Theory(IMaybe<int> m1, string errString, IResult<int, string> expectation)
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
        [MemberData(nameof(GetUnsafelyUnwrapTheoryData))]
        public void UnsafelyUnwrap_Theory(Type t, object maybe, bool isJust, object expectation)
        {
            // arrange - theory
            var unsafe_template = typeof(UnsafeExtensions).GetMethods()
                .Where(m => m.Name == "UnsafelyUnwrap" 
                        && m.GetParameters().Length == 1
                        && m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IMaybe<>))
                .Single();

            var unsafelyUnwrap = unsafe_template.MakeGenericMethod(t);

            // act
            if (isJust)
            {
                var result = unsafelyUnwrap.Invoke(null, new[] { maybe });
                Assert.Equal(expectation, result);
            }
            else
            {
                var ex = Assert.Throws<TargetInvocationException>(() => unsafelyUnwrap.Invoke(null, new[]{ maybe }));
                Assert.IsType<InvalidOperationException>(ex.InnerException);
            }
        }

        [Theory]
        [MemberData(nameof(GetUnwrapTheoryData))]
        public void UnwrapOr_Theory(IMaybe<int> m1, int defaultValue, int expectation)
        {
            // arrange - theory
            // act
            var result = m1.UnwrapOr(defaultValue);

            // assert
            Assert.Equal(expectation, result);
        }

        [Theory]
        [MemberData(nameof(GetUnwrapTheoryData))]
        public void UnwrapOrElse_Theory(IMaybe<int> m1, int defaultValue, int expectation)
        {
            // arrange - theory
            // act
            var result = m1.UnwrapOrElse(() => defaultValue);

            // assert
            Assert.Equal(expectation, result);
        }

        #endregion

        #region Equality Tests
        [Theory]
        [MemberData(nameof(GetEqualityTheoryData1Param))]
        public void Equals_Self_Theory(object m)
        {
            // arrange

            // act
            var result = m.Equals(m);

            // assert
            Assert.True(result);
        }

        [Fact]
        public void Equals_Null_False()
        {
            // arrange
            var m = Maybe.Of(7);
            
            // act
            var r = m.Equals(null);

            Assert.False(r);
        }

        [Theory]
        [MemberData(nameof(GetEqualityTheoryData2Params))]
        public void Equals_XY_Theory(object m1, object m2)
        {
            // arrange
            // act
            var r1 = m1.Equals(m2);
            var r2 = m2.Equals(m1);

            // assert
            Assert.Equal(r1, r2);
        }

        [Theory]
        [MemberData(nameof(GetTransitiveEqualityTheoryData))]
        public void Equals_Transitive_Theory(object m1, object m2, object m3)
        {
            // arrange
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
        public void Equals_Idempotency_Theory(object m1, object m2, bool expectation)
        {
            // arrange
            // act
            for (var i = 0; i < 100; i++)
            {
                var result = m1.Equals(m2);
                Assert.Equal(expectation, result);
            }
        }
        #endregion

        #region IComparable Tests
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void IComparable_AtoA_Zero(bool useGenerics)
        {
            // arrange
            var a = Maybe.Of(8);
            var a_obj = a as object;

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
            var a = Maybe.Of(0);
            var a_obj = a as object;
            var b = Maybe.Of(0);
            var b_obj = b as object;

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
            var a = Maybe.Of(0);
            var a_obj = a as object;
            var b = Maybe.Of(0);
            var b_obj = b as object;
            var c = Maybe.Of(0);
            var c_obj = c as object;
            
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
            var a = Maybe.Of(0);
            var a_obj = a as object;
            var b = Maybe.Of(1);
            var b_obj = b as object;

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
            var a = Maybe.Of(0);
            var a_obj = a as object;
            var b = Maybe.Of(1);
            var b_obj = b as object;
            var c = Maybe.Of(2);
            var c_obj = c as object;

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
        #endregion
    }
}