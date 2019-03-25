using System;
using System.Linq;
using Xunit;

namespace TrueMyth.Test
{
    using TrueMyth.Algebraic;
    using TrueMyth.Unsafe;

    using Maybe = TrueMyth.Algebraic.Maybe;
    using Result = TrueMyth.Algebraic.Result;
    using SimpleResult = TrueMyth.Algebraic.IResult<int,string>;

    public partial class IResultTests
    {
        #region Instance Method Tests
        [Theory]
        [MemberData(nameof(GetAndTheoryData))]
        public void And_Theory(SimpleResult result, IResult<double,string> andResult, object expectation)
        {
            // arrange
            // act
            var r = result.And(andResult);

            // assert
            Assert.Equal(expectation, r);
        }

        [Theory]
        [MemberData(nameof(GetAndTheoryData))]
        public void AndThen_Theory(SimpleResult result, IResult<double,string> andResult, object expectation)
        {
            // arrange
            Func<int, IResult<double,string>> andFn = (i) => andResult;

            // act 
            var r = result.AndThen(andFn);

            Assert.Equal(expectation, r);
        }

        [Theory]
        [MemberData(nameof(GetMapTheoryData))]
        public void Map_Theory(SimpleResult result, double mappedValue, object expectation)
        {
            // arrange
            Func<int,double> mapFn = (i) => mappedValue;

            // act
            var r = result.Map(mapFn);

            // assert
            Assert.Equal(expectation, r);
        }

        [Theory]
        [MemberData(nameof(GetMapReturnTheoryData))]
        public void MapReturn_Theory(SimpleResult result, double mappedValue, double defaultValue, double expectation)
        {
            // arrange
            Func<int,double> mapFn = (i) => mappedValue;

            // act
            var val = result.MapReturn(mapFn, defaultValue);

            // assert
            Assert.Equal(expectation, val);
        }

        [Theory]
        [MemberData(nameof(GetMapErrTheoryData))]
        public void MapErr_Theory(SimpleResult result, int mappedErr, IResult<int,int> expectation)
        {
            // arrange
            Func<string,int> mapFn = s => mappedErr;

            // act
            var r = result.MapErr(mapFn);

            // assert
            Assert.Equal(expectation, r);
        }

        [Theory]
        [MemberData(nameof(GetMatchTheoryData))]
        public void Match_Theory(SimpleResult result, double okMapping, double errMapping, double expectation)
        {
            // arrange
            Func<int, double> okMap = i => okMapping;
            Func<string, double> errMap = s => errMapping;

            // act
            var r = result.Match(okMap, errMap);

            // assert
            Assert.Equal(expectation, r);
        }

        #pragma warning disable xUnit1026
        [Theory]
        [MemberData(nameof(GetMatchTheoryData))]
        public void Match_Void_Theory(SimpleResult result, double okMapping, double errMapping, double expectation)
        {
            // arrange
            // act
            // assert
            result.Match(i => Assert.IsType<Ok<int,string>>(result), s => Assert.IsType<Err<int,string>>(result));
        }
        #pragma warning restore xUnit1026

        [Theory]
        [MemberData(nameof(GetOrTheoryData))]
        public void Or_Theory(SimpleResult result, SimpleResult defaultResult, object expectation)
        {
            // arrange
            // act
            var r = result.Or(defaultResult);

            // assert
            Assert.Equal(expectation, r);
        }

        [Theory]
        [MemberData(nameof(GetOrTheoryData))]
        public void OrElse_Theory(SimpleResult result, SimpleResult elseResult, object expectation)
        {
            // arrange
            Func<SimpleResult> elseFn = () => elseResult;

            // act
            var r = result.OrElse(elseFn);

            // assert
            Assert.Equal(expectation, r);
        }

        [Theory]
        [MemberData(nameof(GetMapTheoryData))]
        public void Select_Theory(SimpleResult result, double mappedValue, object expectation)
        {
            // arrange
            Func<int,double> mapFn = (i) => mappedValue;

            // act
            var r = result.Select(mapFn);

            // assert
            Assert.Equal(expectation, r);
        }

        [Theory]
        [MemberData(nameof(GetMapErrTheoryData))]
        public void SelectErr_Theory(SimpleResult result, int mappedErr, IResult<int,int> expectation)
        {
            // arrange
            Func<string,int> mapFn = s => mappedErr;

            // act
            var r = result.MapErr(mapFn);

            // assert
            Assert.Equal(expectation, r);
        }

        [Theory]
        [MemberData(nameof(GetAndTheoryData))]
        public void SelectMany_Theory(SimpleResult result, IResult<double,string> andResult, object expectation)
        {
            // arrange
            Func<int, IResult<double,string>> andFn = (i) => andResult;

            // act 
            var r = result.SelectMany(andFn);

            Assert.Equal(expectation, r);
        }

        [Theory]
        [MemberData(nameof(GetToMaybeTheoryData))]
        public void ToMaybe_Theory(SimpleResult result, IMaybe<int> expectation)
        {
            // arrange
            // act
            var maybe = result.ToMaybe();
            
            // assert
            Assert.Equal(expectation, maybe);
        }

        [Theory]
        [MemberData(nameof(GetUnwrapTheoryData))]
        public void UnwrapOr_Theory(SimpleResult result, int defaultValue, int expectation)
        {
            // arrange
            // act
            var val = result.UnwrapOr(defaultValue);

            // assert
            Assert.Equal(expectation, val);
        }

        [Theory]
        [MemberData(nameof(GetUnwrapTheoryData))]
        public void UnwrapOrElse_Theory(SimpleResult result, int defaultValue, int expectation)
        {
            // arrange
            Func<string,int> elseFn = s => defaultValue;

            // act
            var val = result.UnwrapOrElse(elseFn);

            // assert
            Assert.Equal(expectation, val);
        }

        #endregion

        #region Object Override Tests
        [Theory]
        [MemberData(nameof(GetEqualPairs))]
        public void Equals_Same_True(SimpleResult r1, SimpleResult r2)
        {
            // arrange
            // act
            var equalityResult = r1.Equals(r2);

            // assert
            Assert.True(equalityResult);
        }

        [Theory]
        [MemberData(nameof(GetUnequalPairs))]
        public void Equals_Different_False(SimpleResult r1, SimpleResult r2)
        {
            // arrange - in theory
            // act
            var equalityResult = r1.Equals(r2);

            // assert
            Assert.False(equalityResult);
        }

        // GetHashCode
        [Theory]
        [MemberData(nameof(GetEqualPairs))]
        public void GetHashCode_SameResults_Equal(SimpleResult r1, SimpleResult r2)
        {
            // arrange - in theory
            // act
            var hc1 = r1.GetHashCode();
            var hc2 = r2.GetHashCode();

            // assert
            Assert.Equal(hc1, hc2);
        }

        [Theory]
        [MemberData(nameof(GetUnequalPairs))]
        public void GetHashCode_DifferentResult_NotEqual(SimpleResult r1, SimpleResult r2)
        {
            // arrange - in theory
            // act
            var hc1 = r1.GetHashCode();
            var hc2 = r2.GetHashCode();

            // assert
            Assert.NotEqual(hc1, hc2);
        }
        #endregion

        #region IComparable Tests
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void IComparable_Ok_AtoA_Zero(bool useGenerics)
        {
            // arrange
            var a = Result.Ok<int,string>(0);
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
        public void IComparable_Err_AtoA_Zero(bool useGenerics)
        {
            // arrange
            var a = Result.Err<int,string>("test");
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
        public void IComparable_Ok_Symmetry_Ok(bool useGenerics)
        {
            // arrange
            var a = Result.Ok<int,string>(0);
            var a_obj = (object)a;
            var b = Result.Ok<int,string>(0);
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
        public void IComparable_Err_Symmetry_Ok(bool useGenerics)
        {
            // arrange
            var a = Result.Err<int,string>("test");
            var a_obj = (object)a;
            var b = Result.Err<int,string>("test");
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
        public void IComparable_Ok_Transitive_Ok(bool useGenerics)
        {
            // arrange
            var a = Result.Ok<int,string>(0);
            var a_obj = (object)a;
            var b = Result.Ok<int,string>(0);
            var b_obj = (object)b;
            var c = Result.Ok<int,string>(0);
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
        public void IComparable_Err_Transitive_Ok(bool useGenerics)
        {
            var a = Result.Err<int,string>("test");
            var b = Result.Err<int,string>("test");
            var c = Result.Err<int,string>("test");
            var a_obj = (object)a;
            var b_obj = (object)b;
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
        public void IComparable_Ok_Inverse_Ok(bool useGenerics)
        {
            var a = Result.Ok<int,string>(0);
            var b = Result.Ok<int,string>(1);
            var a_obj = (object)a;
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
        public void IComparable_Err_Inverse_Ok(bool useGenerics)
        {
            var a = Result.Err<int,string>("test");
            var b = Result.Err<int,string>("xyz");
            var a_obj = (object)a;
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
        public void IComparable_Ok_Inductive_Ok(bool useGenerics)
        {
            var a = Result.Ok<int,string>(0);
            var b = Result.Ok<int,string>(1);
            var c = Result.Ok<int,string>(2);
            var a_obj = (object)a;
            var b_obj = (object)b;
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

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void IComparable_Err_Inductive_Ok(bool useGenerics)
        {
            var a = Result.Ok<int,string>(0);
            var b = Result.Ok<int,string>(1);
            var c = Result.Ok<int,string>(2);
            var a_obj = (object)a;
            var b_obj = (object)b;
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
        #endregion
    }
}