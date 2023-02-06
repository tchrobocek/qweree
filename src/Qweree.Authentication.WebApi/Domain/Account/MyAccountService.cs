using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Qweree.AspNet.Application;
using Qweree.AspNet.Validations;
using Qweree.Authentication.Sdk.Session;
using Qweree.Authentication.WebApi.Domain.Authentication;
using Qweree.Authentication.WebApi.Domain.Identity;
using Qweree.Authentication.WebApi.Domain.Security;
using Qweree.Authentication.WebApi.Domain.Session;
using Qweree.Mongo.Exception;
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
    private readonly ISessionInfoRepository _sessionInfoRepository;
    private readonly IValidator _validator;
    private readonly IClientRepository _clientRepository;
    private readonly AuthenticationService _authenticationService;

    public MyAccountService(IUserRepository userRepository, ISessionStorage sessionStorage,
        IPasswordEncoder passwordEncoder, IDateTimeProvider dateTimeProvider, IValidator validator,
        ISessionInfoRepository sessionInfoRepository, IClientRepository clientRepository,
        AuthenticationService authenticationService)
    {
        _userRepository = userRepository;
        _sessionStorage = sessionStorage;
        _passwordEncoder = passwordEncoder;
        _dateTimeProvider = dateTimeProvider;
        _validator = validator;
        _sessionInfoRepository = sessionInfoRepository;
        _clientRepository = clientRepository;
        _authenticationService = authenticationService;
    }

    public async Task<Response> ChangeMyPasswordAsync(ChangeMyPasswordInput input,
        CancellationToken cancellationToken = new())
    {
        var validationResult = await _validator.ValidateAsync(input, cancellationToken);
        if (validationResult.HasFailed)
            return validationResult.ToErrorResponse();

        User user;
        var id = _sessionStorage.UserId;

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

    public async Task<Response<MyProfile>> GetMeAsync(CancellationToken cancellationToken = new())
    {
        try
        {
            var item = await _userRepository.GetAsync(_sessionStorage.UserId, cancellationToken);
            return Response.Ok(new MyProfile(item.Id, item.Username, item.ContactEmail,
                item.Properties.Select(p => new UserProperty(p.Key, p.Value)).ToImmutableArray()));
        }
        catch (DocumentNotFoundException)
        {
            return Response.Fail<MyProfile>(new Error("User was not found.", StatusCodes.Status404NotFound));
        }
    }

    public async Task<CollectionResponse<SessionInfo>> FindMySessions(CancellationToken cancellationToken = new())
    {
        var items = await _sessionInfoRepository.FindActiveSessionsForUser(_sessionStorage.UserId, cancellationToken);
        return Response.Ok(items);
    }

    public async Task<Response> RevokeAsync(Guid sessionId, CancellationToken cancellationToken = new())
    {
        try
        {
            var session = await _sessionInfoRepository.GetAsync(sessionId, cancellationToken);

            if (_sessionStorage.UserId != session.UserId)
                return Response.Fail("Session does not exist", StatusCodes.Status404NotFound);
        }
        catch (DocumentNotFoundException)
        {
            return Response.Fail("Session does not exist", StatusCodes.Status404NotFound);
        }

        await _sessionInfoRepository.DeleteOneAsync(sessionId, cancellationToken);
        return Response.Ok();
    }

    public async Task<Response<Client>> ApplicationConsentInfoGetAsync(string clientId, CancellationToken cancellationToken = new())
    {
        Client client;

        try
        {
            client = await _clientRepository.GetByClientIdAsync(clientId, cancellationToken);
        }
        catch (DocumentNotFoundException)
        {
            return Response.Fail<Client>("Client does not exist.", StatusCodes.Status404NotFound);
        }

        return Response.Ok(client);
    }

    public async Task<Response<TokenInfo>> ApplicationConsentAsync(string clientId, string redirectUri, string ipAddress,
        UserAgentInfo? userAgentInfo, CancellationToken cancellationToken = new())
    {
        return await _authenticationService.AuthenticateAsync(new ImplicitGrantInput(clientId, redirectUri), ipAddress, userAgentInfo, cancellationToken);
    }
}