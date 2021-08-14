using System;
using System.Threading;
using System.Threading.Tasks;
using Qweree.AspNet.Application;
using Qweree.AspNet.Session;
using Qweree.Authentication.Sdk.Account;
using Qweree.Authentication.WebApi.Domain.Identity;
using Qweree.Authentication.WebApi.Domain.Security;
using Qweree.Utils;
using User = Qweree.Authentication.WebApi.Domain.Identity.User;

namespace Qweree.Authentication.WebApi.Domain.Account
{
    public class MyAccountService
    {
        private readonly ISessionStorage _sessionStorage;
        private readonly IUserRepository _userRepository;
        private readonly IPasswordEncoder _passwordEncoder;
        private readonly IDateTimeProvider _dateTimeProvider;

        public MyAccountService(IUserRepository userRepository, ISessionStorage sessionStorage, IPasswordEncoder passwordEncoder, IDateTimeProvider dateTimeProvider)
        {
            _userRepository = userRepository;
            _sessionStorage = sessionStorage;
            _passwordEncoder = passwordEncoder;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<Response> ChangeMyPasswordAsync(ChangeMyPasswordInput input,
            CancellationToken cancellationToken = new())
        {
            User user;
            var id = _sessionStorage.Id;

            try
            {
                user = await _userRepository.GetAsync(id, cancellationToken);
            }
            catch (Exception)
            {
                return Response.Fail(new Error("User was not found.", 404));
            }

            if (!_passwordEncoder.VerifyPassword(user.Password, input.OldPassword))
            {
                return Response.Fail(new Error("Password does not match.", 400));
            }

            var passwordHash = _passwordEncoder.EncodePassword(input.NewPassword);

            user = new User(user.Id, user.Username, user.FullName, user.ContactEmail, passwordHash, user.Roles,
                user.CreatedAt, _dateTimeProvider.UtcNow);

            await _userRepository.ReplaceAsync(id.ToString(), user, cancellationToken);

            return Response.Ok();
        }
    }
}