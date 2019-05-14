using System;
using TrueMyth;

namespace Samples
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
        Deactivated
    }

    public class UserRepositoryError
    {
        public string Message { get; set; }
        public Maybe<Exception> Exception { get; set; }
        public Maybe<UserRepositoryErrorCategory> ErrorCategory { get; set; }
    }

    public class UserRepository
    {
        private readonly Random _rand = new Random(unchecked((int)DateTime.Now.Ticks));

        public Result<User, UserRepositoryError> GetActiveUser(int id)
        {
            var user = FindUser(id);
            if (user == null) {
                return new UserRepositoryError {
                    Message = "User not found.",
                    ErrorCategory = Maybe.Of(UserRepositoryErrorCategory.NotFound)
                };
            }

            if (user.Deactivated) {
                return new UserRepositoryError {
                    Message = "User was deactivated.",
                    ErrorCategory = Maybe.Of(UserRepositoryErrorCategory.Deactivated)
                };
            }

            return user;
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