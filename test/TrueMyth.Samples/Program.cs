using System;

namespace Samples
{
    using TM = TrueMyth;
    using TMA = TrueMyth.Algebraic;
    using MResult = TrueMyth.Result<bool, ApplicationError>;
    using AResult = TrueMyth.Algebraic.IResult<bool, ApplicationError>;

    public class ApplicationError
    {
        public string Message { get; set; }
        public string Area { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            DoMonadic();
            DoAlgebraic();
        }

        public static MResult DoMonadic()
        {
            var repo = new UserRepository();

            var result = repo.GetActiveUser(1234);
            return result.Match<MResult>(
                user => true, 
                err => new ApplicationError
                {
                    Message = err.Message,
                    Area = "DoMonadic"
                }
            );
        }

        public static AResult DoAlgebraic()
        {
            var repo = new Algebraic.UserRepository();

            var result = repo.GetActiveUser(1234);
            return result.Match(
                user => TMA.Result.Ok<bool, ApplicationError>(true),
                err => TMA.Result.Err<bool, ApplicationError>(new ApplicationError
                {
                    Message = err.Message,
                    Area = "DoAlgebraic"
                })
            );
        }
    }
}
