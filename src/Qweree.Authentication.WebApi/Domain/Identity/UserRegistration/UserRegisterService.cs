using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Qweree.AspNet.Application;
using Qweree.AspNet.Validations;
using Qweree.Authentication.AdminSdk.Identity.Users.UserRegister;
using Qweree.Authentication.Sdk.Account;
using Qweree.Authentication.WebApi.Domain.Security;
using Qweree.Mongo.Exception;
using Qweree.Utils;
using Qweree.Validator;

namespace Qweree.Authentication.WebApi.Domain.Identity.UserRegistration;

public class UserRegisterService
{
    private readonly IValidator _validator;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUserInvitationRepository _userInvitationRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPasswordEncoder _passwordEncoder;

    public UserRegisterService(IValidator validator, IDateTimeProvider dateTimeProvider,
        IUserInvitationRepository userInvitationRepository, IUserRepository userRepository, IPasswordEncoder passwordEncoder)
    {
        _validator = validator;
        _dateTimeProvider = dateTimeProvider;
        _userInvitationRepository = userInvitationRepository;
        _userRepository = userRepository;
        _passwordEncoder = passwordEncoder;
    }

    public async Task<Response> RegisterAsync(UserRegisterInput input, CancellationToken cancellationToken = new())
    {
        UserInvitationDescriptor invitationDescriptor;

        try
        {
            invitationDescriptor = await _userInvitationRepository.GetAsync(input.UserInvitationId, cancellationToken);
        }
        catch (DocumentNotFoundException)
        {
            return Response.Fail("Invitation was not found.");
        }

        if (invitationDescriptor.ExpiresAt < _dateTimeProvider.UtcNow)
            return Response.Fail("Invitation was not found.");

        input = new UserRegisterInput(invitationDescriptor.Id, invitationDescriptor.Username ?? input.Username,
            invitationDescriptor.FullName ?? input.Fullname, invitationDescriptor.ContactEmail ?? input.ContactEmail, input.Password);

        var result = await _validator.ValidateAsync(input, cancellationToken);
        if (result.HasFailed)
            return result.ToErrorResponse();

        var user = new User(Guid.NewGuid(), input.Username, input.Fullname, input.ContactEmail,
            _passwordEncoder.EncodePassword(input.Password), invitationDescriptor.Roles ?? ImmutableArray<Guid>.Empty, _dateTimeProvider.UtcNow,
            _dateTimeProvider.UtcNow);

        await _userRepository.InsertAsync(user, cancellationToken);
        await _userInvitationRepository.DeleteAsync(invitationDescriptor.Id, cancellationToken);

        return Response.Ok();
    }
}