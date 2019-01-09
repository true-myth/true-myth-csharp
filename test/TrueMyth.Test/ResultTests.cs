using System;
using Xunit;
using TrueMyth;

namespace TrueMyth.Test
{
    using SimpleResult = Result<int, string>;

    public partial class ResultTests
    {
        [Fact]
        public void Implicit_Ok_OK()
        {
            // act
            var result = SimpleResult.Ok(7);

            // assert
            Assert.True(result.IsOk);
            Assert.False(result.IsErr);
        }

        [Fact]
        public void Implicit_Error_OK()
        {
            // act
            var result = SimpleResult.Err("message");

            // assert
            Assert.True(result.IsErr);
            Assert.False(result.IsOk);
        }

        [Fact]
        public void Explicit_Value_Ok()
        {
            // act
            var result = SimpleResult.Ok(7);

            // assert
            Assert.True(result.IsOk);
            Assert.False(result.IsErr);
            Assert.Equal<int>(7, (int)result);
        }

        [Fact]
        public void Explicit_Error_Ok()
        {
            // act
            var result = SimpleResult.Err("message");

            // assert
            Assert.True(result.IsErr);
            Assert.False(result.IsOk);
            Assert.Equal("message", (string)result);
        }

        [Fact]
        public void Explicit_NullError_Ok()
        {
            // act
            var result = SimpleResult.Err(null);

            // assert
            Assert.True(result.IsErr);
            Assert.False(result.IsOk);
            Assert.Null(result.UnsafelyUnwrapErr());
        }

        [Fact]
        public void Implicit_ReturnOk_Ok()
        {
            SimpleResult f() => 7;

            // act
            var result = f();

            // assert
            Assert.True(result.IsOk);
            Assert.False(result.IsErr);
            Assert.Equal<int>(7, (int)result);
        }

        [Fact]
        public void Implicit_ReturnErr_Ok()
        {
            SimpleResult f() => "something bad";

            // act
            var result = f();

            // assert
            Assert.True(result.IsErr);
            Assert.False(result.IsOk);
            Assert.Equal("something bad", result);
        }

        [Fact]
        public void UnwrapError_AsValue_Throws()
        {
            // arrange
            var result = SimpleResult.Err("something bad happened");

            // act
            // assert
            Assert.True(result.IsErr);
            Assert.False(result.IsOk);
            Assert.Throws<InvalidOperationException>(() => result.UnsafelyUnwrap());
        }

        [Fact]
        public void UnwrapOk_AsError_Throw()
        {
            var result = SimpleResult.Ok(8);

            // act
            // assert
            Assert.True(result.IsOk);
            Assert.False(result.IsErr);
            Assert.Throws<InvalidOperationException>(() => result.UnsafelyUnwrapErr());
        }

        [Fact]
        public void And_OkReturnsAndResult_Ok()
        {
            // arrange
            var result = SimpleResult.Ok(7);
            var andResult = SimpleResult.Ok(1);

            // act
            var r = result.And(andResult);

            // assert
            Assert.Equal(andResult, r);
        }

        [Fact]
        public void And_ErrReturnsThis_Ok()
        {
            // arrange
            var result = SimpleResult.Err("error");
            var andResult = SimpleResult.Ok(0);

            // act
            var r = result.And(andResult);

            // assert
            Assert.Equal(result, r);
        }

        // Match
        [Fact]
        public void Match_OkCallsOk_Ok()
        {
            // arrange
            var result = SimpleResult.Ok(7);

            // act
            var functionCalled = result.Match(
                ok: value => "ok",
                err: error => "error"
            );

            // Assert
            Assert.Equal("ok", functionCalled);
        }

        [Theory]
        [MemberData(nameof(GetResultTheoryData1))]
        public void Match_Void_Theory(SimpleResult r)
        {
            r.Match(
                ok: _ => Assert.True(r.IsOk),
                err: _ => Assert.True(r.IsErr)
            );
        }

        // Or
        [Fact]
        public void Or_OkReturnsThis_Ok()
        {
            // arrange
            var r1 = SimpleResult.Ok(0);
            var r2 = SimpleResult.Ok(1);

            // act
            var r = r1.Or(r2);

            // assert
            Assert.Equal(r1, r);
        }

        [Fact]
        public void Or_ErrReturnsThat_Ok()
        {
            // arrange
            var r1 = SimpleResult.Err("error");
            var r2 = SimpleResult.Ok(0);
            
            // act
            var r = r1.Or(r2);

            // assert
            Assert.Equal(r2, r);
        }

        // OrElse
        [Fact]
        public void OrElse_Ok()
        {
            // arrange
            var r1 = SimpleResult.Ok(0);
            var r2 = SimpleResult.Ok(1);

            // act
            var r = r1.OrElse(() => r2);

            // assert
            Assert.Equal<SimpleResult>(r1, r);
        }

        [Fact]
        public void OrElse_Err()
        {
            // arrange
            var r1 = SimpleResult.Err("error");
            var r2 = SimpleResult.Ok(0);

            // act
            var r = r1.OrElse(() => r2);

            // assert
            Assert.Equal<SimpleResult>(r2, r);
        }

        // Select
        [Fact]
        public void Select_Ok()
        {
            // arrange
            var r1 = SimpleResult.Ok(0);

            // act
            var r = r1.Select(value => value.ToString());

            // assert
            Assert.NotNull(r);
            Assert.True(r.IsOk);
            Assert.Equal("0", r.UnsafelyUnwrap());
        }

        [Fact]
        public void SelectErr_Ok()
        {
            // arrange
            var r1 = SimpleResult.Err("error");

            // act
            var r = r1.SelectErr(error => error.ToUpperInvariant());

            // assert
            Assert.NotNull(r);
            Assert.True(r.IsErr);
            Assert.Equal("ERROR", r.UnsafelyUnwrapErr());
        }

        [Fact]
        public void  SelectErrOk_Ok()
        {
            // arrange
            var r1 = SimpleResult.Ok(0);

            // act
            var r = r1.SelectErr(error => Convert.ToInt16(error));

            // assert
            Assert.NotNull(r);
            Assert.True(r.IsOk);
        }

        [Fact]
        public void Bind_Ok()
        {
            // arrange
            var r1 = SimpleResult.Ok(0);

            // act
            var r = r1.AndThen(i => Result<float, string>.Ok(3.7f));

            // assert
            Assert.NotNull(r);
            Assert.True(r.IsOk);
            Assert.Equal(3.7f, r.UnsafelyUnwrap());
        }

        [Fact]
        public void Bind_Err_Ok()
        {
            // arrange
            var r1 = SimpleResult.Err("error");

            // act
            var r = r1.AndThen(i => Result<float,string>.Ok(3.7f));

            // assert
            Assert.NotNull(r);
            Assert.True(r.IsErr);
            Assert.Equal("error", r.UnsafelyUnwrapErr());
        }

        [Fact]
        public void MapReturnOk_Ok()
        {
            // arrange
            var r1 = SimpleResult.Ok(0);

            // act
            var r = r1.MapReturn(i => i.ToString(), "default");

            // assert
            Assert.NotNull(r);
            Assert.Equal("0", r);
        }

        [Fact]
        public void MapReturn_Err_Ok()
        {
            // arrange
            var r1 = SimpleResult.Err("error");

            // act
            var r = r1.MapReturn(i => i.ToString(), "default");

            // assert
            Assert.NotNull(r);
            Assert.Equal("default", r);
        }

        // ToString
        [Theory]
        [MemberData(nameof(GetToStringTests))]
        public void ToString_Ok(SimpleResult r1, string expectedString)
        {
            // arrange - in theory
            // act
            var r = r1.ToString();

            // assert
            Assert.NotNull(r);
            Assert.Equal(expectedString, r);
        }

        // UnwrapOr
        [Fact]
        public void UnwrapOr_Ok_Ok()
        {
            // arrange
            var r1 = SimpleResult.Ok(7);

            // act
            var r = r1.Unwrap(0);

            // assert
            Assert.Equal(7, r);
        }

        [Fact]
        public void UnwrapOr_Err_Ok()
        {
            // arrange
            var r1  = SimpleResult.Err("error");

            // act
            var r = r1.Unwrap(0);

            // assert
            Assert.Equal(0, r);
        }

        // UnwrapOrElse        
        [Fact]
        public void UnwrapOrElse_Ok_Ok()
        {
            // arrange
            var r1 = SimpleResult.Ok(7);

            // act
            var r = r1.Unwrap(err => 0);

            // assert
            Assert.Equal(7, r);
        }

        [Fact]
        public void UnwrapOrElse_Err_Ok()
        {
            // arrange
            var r1 = SimpleResult.Err("error");

            // act
            var r = r1.Unwrap(err => 0);

            // assert
            Assert.Equal(0, r);
        }

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

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void IComparable_Ok_AtoA_Zero(bool useGenerics)
        {
            // arrange
            var a = SimpleResult.Ok(0);
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
            var a = SimpleResult.Err("test");
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
            var a = SimpleResult.Ok(0);
            var a_obj = (object)a;
            var b = SimpleResult.Ok(0);
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
            var a = SimpleResult.Err("test");
            var a_obj = (object)a;
            var b = SimpleResult.Err("test");
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
            var a = SimpleResult.Ok(0);
            var a_obj = (object)a;
            var b = SimpleResult.Ok(0);
            var b_obj = (object)b;
            var c = SimpleResult.Ok(0);
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
            var a = SimpleResult.Err("test");
            var b = SimpleResult.Err("test");
            var c = SimpleResult.Err("test");
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
            var a = SimpleResult.Ok(0);
            var b = SimpleResult.Ok(1);
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
            var a = SimpleResult.Err("test");
            var b = SimpleResult.Err("xyz");
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
            var a = SimpleResult.Ok(0);
            var b = SimpleResult.Ok(1);
            var c = SimpleResult.Ok(2);
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
            var a = SimpleResult.Ok(0);
            var b = SimpleResult.Ok(1);
            var c = SimpleResult.Ok(2);
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
    }
}
