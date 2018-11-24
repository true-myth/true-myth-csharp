using System;
using BenchmarkDotNet.Attributes;
using TrueMyth;

namespace TrueMyth.Test.Benchmarks
{
    public class ResultCreationBenchmarks
    {
        private readonly Maybe<int> _just = Maybe.Of(7);
        private readonly Maybe<int> _nothing = Maybe<int>.Nothing;

        public ResultCreationBenchmarks()
        {

        }

        [Benchmark]
        public Result<int,string> ResultFactoryOk() => Result<int,string>.Ok(7);

        [Benchmark]
        public Result<int,string> ResultFactoryErr() => Result<int,string>.Err("error");

        [Benchmark]
        public Result<int,string> ResultFromJust() => Result.From(_just, "error");

        [Benchmark]
        public Result<int,string> ResultFromNothing() => Result.From(_nothing, "error");
    }

    public class MaybeCreationBenchmarks
    {
        private readonly Result<int,string> _ok = Result<int,string>.Ok(7);
        private readonly Result<int,string> _err = Result<int,string>.Err("error");

        [Benchmark]
        public Maybe<int> MaybeFactoryOf() => Maybe.Of(7);
    
        [Benchmark]
        public Maybe<int> MaybePropertyNothing() => Maybe<int>.Nothing;

        [Benchmark]
        public Maybe<int> MaybeFromOk() => Maybe.From(_ok);

        [Benchmark]
        public Maybe<int> MaybeFromNothing() => Maybe.From(_err);
    }
}