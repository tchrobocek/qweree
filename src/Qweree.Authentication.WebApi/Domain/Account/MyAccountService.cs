using System;
using System.Threading;
using System.Threading.Tasks;
using Qweree.AspNet.Application;
using Qweree.AspNet.Validations;
using Qweree.Authentication.Sdk.Account;
using Qweree.Authentication.WebApi.Domain.Identity;
using Qweree.Authentication.WebApi.Domain.Security;
using Qweree.Session;
using Qweree.Utils;
using Qweree.Validator;
using User = Qweree.Authentication.WebApi.Domain.Identity.User;

namespace Qweree.Authentication.WebApi.Domain.Account;

public class MyAccountService
{
    private readonly ISessionStorage _sessionStorage;
    private readonly IUserRepository _userRepository;
    private readonly IPasswordEncoder _passwordEncoder;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IValidator _validator;

    public MyAccountService(IUserRepository userRepository, ISessionStorage sessionStorage,
        IPasswordEncoder passwordEncoder, IDateTimeProvider dateTimeProvider, IValidator validator)
    {
        _userRepository = userRepository;
        _sessionStorage = sessionStorage;
        _passwordEncoder = passwordEncoder;
        _dateTimeProvider = dateTimeProvider;
        _validator = validator;
    }

    public async Task<Response> ChangeMyPasswordAsync(ChangeMyPasswordInput input,
        CancellationToken cancellationToken = new())
    {
        var validationResult = await _validator.ValidateAsync(input, cancellationToken);
        if (validationResult.HasFailed)
            return validationResult.ToErrorResponse();

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

        user = new User(user.Id, user.Username, user.FullName, user.ContactEmail, passwordHash, user.Properties, user.Roles,
            user.CreatedAt, _dateTimeProvider.UtcNow);

        await _userRepository.ReplaceAsync(id.ToString(), user, cancellationToken);

        return Response.Ok();
    }
}