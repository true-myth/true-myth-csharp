using System;
using TrueMyth.Algebraic;

namespace Samples.Algebraic
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool Deactivated { get; set; }
    }

    public enum UserRepositoryErrorCategory
    {
        NotFound,
        Deactivated,
    }

    public class UserRepositoryError
    {
        public string Message { get; set; }
        public IMaybe<Exception> Exception { get; set; }
        public IMaybe<UserRepositoryErrorCategory> ErrorCategory { get; set; }
    }

    public class UserRepository
    {
        private readonly Random _rand = new Random(unchecked((int)DateTime.Now.Ticks));

        public IResult<User,UserRepositoryError> GetActiveUser(int id)
        {
            var user = FindUser(id);
            if (user == null) {
                return Result.Err<User,UserRepositoryError>(new UserRepositoryError {
                    Message = "User not found.",
                    ErrorCategory = Maybe.Of(UserRepositoryErrorCategory.NotFound)
                });
            }

            if (user.Deactivated) {
                return Result.Err<User,UserRepositoryError>(new UserRepositoryError {
                    Message = "User was deactivated.",
                    ErrorCategory = Maybe.Of(UserRepositoryErrorCategory.Deactivated)
                });
            }

            return Result.Ok<User,UserRepositoryError>(user);
        }

        private User FindUser(int id)
        {
            //mmmmmmmmagic
            return _rand.RandBool() 
                ? new User {
                    Id = _rand.RandBool() ? id : _rand.Next(),
                    Name = "SampleUser",
                    Email = "sample@truemyth.org",
                    Deactivated = _rand.RandBool()
                  }
                : null;
        }
    }
}