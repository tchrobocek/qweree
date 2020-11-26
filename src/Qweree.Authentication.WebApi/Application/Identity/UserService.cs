using System;
using System.Threading;
using System.Threading.Tasks;
using DevOne.Security.Cryptography.BCrypt;
using Qweree.AspNet.Application;
using Qweree.Authentication.WebApi.Domain.Identity;
using Qweree.Mongo.Exception;
using Qweree.Utils;

namespace Qweree.Authentication.WebApi.Application.Identity
{
    public class UserService
    {
        private readonly IDateTimeProvider _datetimeProvider;
        private readonly IUserRepository _userRepository;
        private readonly string _passwordSalt;

        public UserService(IDateTimeProvider datetimeProvider, IUserRepository userRepository, string passwordSalt)
        {
            _datetimeProvider = datetimeProvider;
            _userRepository = userRepository;
            _passwordSalt = passwordSalt;
        }

        public async Task<Response<User>> CreateUserAsync(CreateUserInput createUserInput, CancellationToken cancellationToken = new CancellationToken())
        {
            var password = EncryptPassword(createUserInput.Password);
            var user = new User(Guid.NewGuid(), createUserInput.Username, createUserInput.FullName, createUserInput.ContactEmail,
                password, createUserInput.Roles, _datetimeProvider.UtcNow, _datetimeProvider.UtcNow);

            try
            {
                await _userRepository.InsertAsync(user, cancellationToken);
            }
            catch (InsertDocumentException)
            {
                return Response.Fail<User>("User is duplicate.s");
            }

            return Response.Ok(user);
        }

        private string EncryptPassword(string password)
        {
            if (password == string.Empty)
            {
                return string.Empty;
            }

            return BCryptHelper.HashPassword(password, _passwordSalt);
        }
    }
}