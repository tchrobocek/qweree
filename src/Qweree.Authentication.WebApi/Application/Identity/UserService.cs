using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Qweree.AspNet.Application;
using Qweree.Authentication.Sdk.Identity;
using Qweree.Authentication.WebApi.Domain.Identity;
using Qweree.Mongo.Exception;
using Qweree.Utils;
using Qweree.Validator;
using User = Qweree.Authentication.WebApi.Domain.Identity.User;

namespace Qweree.Authentication.WebApi.Application.Identity
{
    public class UserService
    {
        private readonly IDateTimeProvider _datetimeProvider;
        private readonly IUserRepository _userRepository;
        private readonly IValidator _validator;

        public UserService(IDateTimeProvider datetimeProvider, IUserRepository userRepository, IValidator validator)
        {
            _datetimeProvider = datetimeProvider;
            _userRepository = userRepository;
            _validator = validator;
        }

        public async Task<Response<User>> CreateUserAsync(UserCreateInput userCreateInput, CancellationToken cancellationToken = new CancellationToken())
        {
            var validationResult = await _validator.ValidateAsync(userCreateInput, cancellationToken);
            if (validationResult.HasFailed)
                return Response.Fail<User>(validationResult.Errors.Select(e => $"{e.Path} - {e.Message}."));

            var password = EncryptPassword(userCreateInput.Password);
            var user = new User(Guid.NewGuid(), userCreateInput.Username, userCreateInput.FullName, userCreateInput.ContactEmail,
                password, userCreateInput.Roles, _datetimeProvider.UtcNow, _datetimeProvider.UtcNow);

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

            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}