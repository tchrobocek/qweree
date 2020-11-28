using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Authentication.WebApi.Domain.Identity;
using Qweree.Authentication.WebApi.Infrastructure.Validations;
using Qweree.Mongo;
using Qweree.Mongo.Exception;

namespace Qweree.Authentication.WebApi.Infrastructure.Identity
{
    public class UserRepository : MongoRepositoryBase<User, UserDo>, IUserRepository, IUniqueConstraintValidatorRepository
    {
        public UserRepository(MongoContext context) : base("users", context)
        {
        }

        protected override Func<User, UserDo> ToDocument => UserMapper.ToDo;
        protected override Func<UserDo, User> FromDocument => UserMapper.FromDo;

        public async Task<User> GetByUsernameAsync(string username, CancellationToken cancellationToken = new CancellationToken())
        {
            var user = (await FindAsync($@"{{""Username"": ""{username}""}}", 0, 1, cancellationToken))
                .FirstOrDefault();

            if (user == null)
                throw new DocumentNotFoundException(@$"User ""{username}"" was not found.");

            return user;
        }

        public async Task<bool> IsExistingAsync(string field, string value, CancellationToken cancellationToken = new CancellationToken())
        {
            var user = (await FindAsync($@"{{""{field}"": ""{value}""}}", 0, 1, cancellationToken))
                .FirstOrDefault();

            return user != null;
        }
    }
}