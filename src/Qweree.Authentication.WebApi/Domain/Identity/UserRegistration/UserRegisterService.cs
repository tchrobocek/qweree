using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Qweree.AspNet.Application;
using Qweree.AspNet.Validations;
using Qweree.Authentication.Sdk.Account;
using Qweree.Mongo.Exception;
using Qweree.Utils;
using Qweree.Validator;
using UserInvitation = Qweree.Authentication.AdminSdk.Identity.Users.UserRegister.UserInvitation;

namespace Qweree.Authentication.WebApi.Domain.Identity.UserRegistration
{
    public class UserRegisterService
    {
        private readonly IValidator _validator;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IUserInvitationRepository _userInvitationRepository;
        private readonly IUserRepository _userRepository;

        public UserRegisterService(IValidator validator, IDateTimeProvider dateTimeProvider,
            IUserInvitationRepository userInvitationRepository, IUserRepository userRepository)
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
            catch (DocumentNotFoundException)
            {
                return Response.Fail("Invitation was not found.");
            }

            if (invitation.ExpiresAt < _dateTimeProvider.UtcNow)
                return Response.Fail("Invitation was not found.");

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