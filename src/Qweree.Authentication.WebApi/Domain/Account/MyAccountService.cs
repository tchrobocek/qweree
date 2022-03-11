using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Qweree.AspNet.Application;
using Qweree.AspNet.Validations;
using Qweree.Authentication.Sdk.Account;
using Qweree.Authentication.Sdk.Session;
using Qweree.Authentication.WebApi.Domain.Authentication;
using Qweree.Authentication.WebApi.Domain.Identity;
using Qweree.Authentication.WebApi.Domain.Security;
using Qweree.Mongo.Exception;
using Qweree.Utils;
using Qweree.Validator;
using User = Qweree.Authentication.WebApi.Domain.Identity.User;
using UserProperty = Qweree.Authentication.Sdk.Users.UserProperty;

namespace Qweree.Authentication.WebApi.Domain.Account;

public class MyAccountService
{
    private readonly ISessionStorage _sessionStorage;
    private readonly IUserRepository _userRepository;
    private readonly IPasswordEncoder _passwordEncoder;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IValidator _validator;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public MyAccountService(IUserRepository userRepository, ISessionStorage sessionStorage,
        IPasswordEncoder passwordEncoder, IDateTimeProvider dateTimeProvider, IValidator validator,
        IRefreshTokenRepository refreshTokenRepository)
    {
        _userRepository = userRepository;
        _sessionStorage = sessionStorage;
        _passwordEncoder = passwordEncoder;
        _dateTimeProvider = dateTimeProvider;
        _validator = validator;
        _refreshTokenRepository = refreshTokenRepository;
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

        user = new User(user.Id, user.Username, user.ContactEmail, passwordHash, user.Properties, user.Roles,
            user.CreatedAt, _dateTimeProvider.UtcNow);

        await _userRepository.ReplaceAsync(id.ToString(), user, cancellationToken);

        return Response.Ok();
    }

    public async Task<CollectionResponse<Sdk.Account.DeviceInfo>> FindMyDevicesAsync(CancellationToken cancellationToken = new())
    {
        var items = await _refreshTokenRepository.FindValidForUser(_sessionStorage.Id, cancellationToken);
        var result = items.Where(r => r.Device != null)
            .Select(r => new Sdk.Account.DeviceInfo(r.Id, r.Device!.Client, r.Device.Os, r.Device.Device,
                r.Device.Brand, r.Device.Model, r.CreatedAt, r.ExpiresAt));
        return Response.Ok(result);
    }

    public async Task<Response<MyProfile>> GetMeAsync(CancellationToken cancellationToken = new())
    {
        try
        {
            var item = await _userRepository.GetAsync(_sessionStorage.Id, cancellationToken);
            return Response.Ok(new MyProfile(item.Id, item.Username, item.ContactEmail, item.Properties.Select(p => new UserProperty(p.Key, p.Value)).ToImmutableArray()));
        }
        catch (DocumentNotFoundException)
        {
            return Response.Fail<MyProfile>(new Error("User was not found.", StatusCodes.Status404NotFound));
        }
    }
}