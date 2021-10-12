using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Qweree.AspNet.Application;
using Qweree.Authentication.AdminSdk.Identity.Users.UserRegister;
using Qweree.Authentication.Sdk.Account;
using Qweree.Authentication.WebApi.Domain.Identity;
using Qweree.Authentication.WebApi.Infrastructure.Validations;
using Qweree.Mongo.Exception;
using Qweree.Utils;
using Qweree.Validator;

namespace Qweree.Authentication.WebApi.Infrastructure.Identity.UserRegister
{
    public class UserRegisterService
    {
        private readonly IValidator _validator;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly UserInvitationRepository _userInvitationRepository;
        private readonly UserRepository _userRepository;

        public UserRegisterService(IValidator validator, IDateTimeProvider dateTimeProvider,
            UserInvitationRepository userInvitationRepository, UserRepository userRepository)
        {
            _validator = validator;
            _dateTimeProvider = dateTimeProvider;
            _userInvitationRepository = userInvitationRepository;
            _userRepository = userRepository;
        }

        public async Task<Response> RegisterAsync(UserRegisterInput input, CancellationToken cancellationToken = new())
        {
            UserInvitation invitation;

            try
            {
                invitation = await _userInvitationRepository.GetAsync(input.UserInvitationId, cancellationToken);
            }
            catch (DocumentNotFoundException e)
            {
                return Response.Fail(e.Message);
            }

            input = new UserRegisterInput(invitation.Id, invitation.Username ?? input.Username,
                invitation.FullName ?? input.Fullname, invitation.ContactEmail ?? input.ContactEmail, input.Password);

            var result = await _validator.ValidateAsync(input, cancellationToken);
            if (result.HasFailed)
                return result.ToErrorResponse();

            var user = new User(Guid.NewGuid(), input.Username, input.Fullname, input.ContactEmail,
                input.Password, invitation.Roles ?? ImmutableArray<Guid>.Empty, _dateTimeProvider.UtcNow,
                _dateTimeProvider.UtcNow);

            await _userRepository.InsertAsync(user, cancellationToken);
            await _userInvitationRepository.DeleteAsync(invitation.Id, cancellationToken);

            return Response.Ok();
        }
    }
}