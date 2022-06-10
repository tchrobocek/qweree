using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;
using Qweree.AspNet.Application;
using Qweree.Authentication.Sdk.OAuth2;
using Qweree.Authentication.Sdk.Session.Tokens;
using Qweree.Authentication.WebApi.Domain.Authorization;
using Qweree.Authentication.WebApi.Domain.Authorization.Roles;
using Qweree.Authentication.WebApi.Domain.Identity;
using Qweree.Authentication.WebApi.Domain.Security;
using Qweree.Authentication.WebApi.Domain.Session;
using Qweree.Utils;
using Client = Qweree.Authentication.WebApi.Domain.Identity.Client;
using IdentityUser = Qweree.Authentication.Sdk.Session.IdentityUser;
using User = Qweree.Authentication.WebApi.Domain.Identity.User;
using UserProperty = Qweree.Authentication.Sdk.Users.UserProperty;

namespace Qweree.Authentication.WebApi.Domain.Authentication;

public class AuthenticationService
{
    private const string AccessDeniedMessage = "Access denied.";
    private const int RefreshTokenLength = 16;

    private readonly int _accessTokenValiditySeconds;
    private readonly IDateTimeProvider _datetimeProvider;
    private readonly Random _random;
    private readonly int _refreshTokenValiditySeconds;
    private readonly IUserRepository _userRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IPasswordEncoder _passwordEncoder;
    private readonly AuthorizationService _authorizationService;
    private readonly IClientRoleRepository _clientRoleRepository;
    private readonly ITokenEncoder _tokenEncoder;
    private readonly ISessionInfoRepository _sessionInfoRepository;

    private readonly string RefreshTokenChars = "0123456789abcdefghijklmnopqrstuvwxyz";

    public AuthenticationService(IUserRepository userRepository,
        IDateTimeProvider datetimeProvider, Random random, int accessTokenValiditySeconds, int refreshTokenValiditySeconds, IPasswordEncoder passwordEncoder,
        IClientRepository clientRepository, AuthorizationService authorizationService, IClientRoleRepository clientRoleRepository,
        ITokenEncoder tokenEncoder, ISessionInfoRepository sessionInfoRepository)
    {
        _userRepository = userRepository;
        _datetimeProvider = datetimeProvider;
        _accessTokenValiditySeconds = accessTokenValiditySeconds;
        _passwordEncoder = passwordEncoder;
        _clientRepository = clientRepository;
        _authorizationService = authorizationService;
        _clientRoleRepository = clientRoleRepository;
        _tokenEncoder = tokenEncoder;
        _sessionInfoRepository = sessionInfoRepository;
        _refreshTokenValiditySeconds = refreshTokenValiditySeconds;
        _random = random;
    }

    public async Task<Response<TokenInfo>> AuthenticateAsync(PasswordGrantInput input,
        ClientCredentials clientCredentials, DeviceInfo? device, CancellationToken cancellationToken = new())
    {
        var now = _datetimeProvider.UtcNow;

        User user;
        Client client;

        try
        {
            user = await AuthenticateUserAsync(input, cancellationToken);
            client = await AuthenticateClientAsync(clientCredentials, GrantType.Password, cancellationToken);
        }
        catch (Exception)
        {
            return Response.Fail<TokenInfo>(AccessDeniedMessage);
        }

        var session = await BeginSessionAsync(client, user, device, GrantType.Password, true);

        var effectiveRoles = new List<string>();
        await foreach (var role in _authorizationService.GetEffectiveUserRoles(user, cancellationToken)
                           .WithCancellation(cancellationToken))
        {
            effectiveRoles.Add(role.Key);
        }

        effectiveRoles.Add("USER");

        var expiresAt = now + TimeSpan.FromSeconds(_accessTokenValiditySeconds);
        var identity = new Sdk.Session.Identity(new Sdk.Session.IdentityClient(client.Id, client.ClientId, client.ApplicationName),
            new IdentityUser(user.Id, user.Username, user.Properties.Select(p => new UserProperty(p.Key, p.Value)).ToImmutableArray()),
            user.ContactEmail, effectiveRoles.ToImmutableArray());
        var accessToken = new AccessToken(session.Id, identity, now, expiresAt);
        var jwt = _tokenEncoder.EncodeAccessToken(accessToken);

        var tokenInfo = new TokenInfo(jwt, session.RefreshToken, expiresAt);
        return Response.Ok(tokenInfo);
    }

    public async Task<Response<TokenInfo>> AuthenticateAsync(RefreshTokenGrantInput input,
        ClientCredentials clientCredentials, DeviceInfo? device, CancellationToken cancellationToken = new())
    {
        var now = _datetimeProvider.UtcNow;

        SessionInfo sessionInfo;
        User user;
        Client client;

        try
        {
            sessionInfo = await _sessionInfoRepository.GetByRefreshTokenAsync(input.RefreshToken, cancellationToken);
            user = await _userRepository.GetAsync(sessionInfo.UserId ?? Guid.Empty, cancellationToken);
            client = await AuthenticateClientAsync(clientCredentials, GrantType.RefreshToken, cancellationToken);
        }
        catch (Exception)
        {
            return Response.Fail<TokenInfo>(AccessDeniedMessage);
        }

        if (sessionInfo.ClientId != client.Id)
            return Response.Fail<TokenInfo>(AccessDeniedMessage);

        var session = await RefreshSessionAsync(sessionInfo);

        if (session.ExpiresAt < now)
            return Response.Fail<TokenInfo>(AccessDeniedMessage);

        var effectiveRoles = new List<string>();
        await foreach (var role in _authorizationService.GetEffectiveUserRoles(user, cancellationToken)
                           .WithCancellation(cancellationToken))
        {
            effectiveRoles.Add(role.Key);
        }

        effectiveRoles.Add("USER");

        var expiresAt = now + TimeSpan.FromSeconds(_accessTokenValiditySeconds);
        var identity = new Sdk.Session.Identity(new Sdk.Session.IdentityClient(client.Id, client.ClientId, client.ApplicationName),
            new IdentityUser(user.Id, user.Username,
                user.Properties.Select(p => new UserProperty(p.Key, p.Value)).ToImmutableArray()),
            user.ContactEmail, effectiveRoles.ToImmutableArray());
        var accessToken = new AccessToken(session.Id, identity, now, expiresAt);
        var jwt = _tokenEncoder.EncodeAccessToken(accessToken);

        var tokenInfo = new TokenInfo(jwt, session.RefreshToken, expiresAt);
        return Response.Ok(tokenInfo);
    }

    public async Task<Response<TokenInfo>> AuthenticateAsync(ClientCredentials clientCredentials, DeviceInfo? device, CancellationToken cancellationToken = new())
    {
        var now = _datetimeProvider.UtcNow;

        User owner;
        Client client;

        try
        {
            client = await AuthenticateClientAsync(clientCredentials, GrantType.ClientCredentials, cancellationToken);
            owner = await _userRepository.GetAsync(client.OwnerId, cancellationToken);
        }
        catch (Exception)
        {
            return Response.Fail<TokenInfo>(AccessDeniedMessage);
        }

        var session = await BeginSessionAsync(client, null, device, GrantType.ClientCredentials, false);

        var effectiveRoles = new List<string>();
        await foreach (var role in _authorizationService.GetEffectiveUserRoles(client, cancellationToken)
                           .WithCancellation(cancellationToken))
        {
            effectiveRoles.Add(role.Key);
        }

        effectiveRoles.Add("CLIENT");

        var expiresAt = now + TimeSpan.FromSeconds(_accessTokenValiditySeconds);
        var identity = new Sdk.Session.Identity(new Sdk.Session.IdentityClient(client.Id, client.ClientId, client.ApplicationName),
            owner.ContactEmail, effectiveRoles.ToImmutableArray());
        var accessToken = new AccessToken(session.Id, identity, now, expiresAt);
        var jwt = _tokenEncoder.EncodeAccessToken(accessToken);

        var tokenInfo = new TokenInfo(jwt, null, expiresAt);
        return Response.Ok(tokenInfo);
    }

    private async Task<SessionInfo> BeginSessionAsync(Client client, User? user, DeviceInfo? deviceInfo, GrantType grantType, bool issueRefreshToken)
    {
        var expiresAt  = _datetimeProvider.UtcNow + TimeSpan.FromSeconds(_accessTokenValiditySeconds);
        var refreshToken = string.Empty;

        if (issueRefreshToken)
        {
            expiresAt = _datetimeProvider.UtcNow + TimeSpan.FromSeconds(_refreshTokenValiditySeconds);
            refreshToken = GenerateRefreshToken();
        }

        var session = new SessionInfo(Guid.NewGuid(), client.Id, user?.Id, refreshToken, deviceInfo, grantType,
            _datetimeProvider.UtcNow, _datetimeProvider.UtcNow, expiresAt);

        await _sessionInfoRepository.InsertAsync(session);
        
        return session;
    }

    private async Task<SessionInfo> RefreshSessionAsync(SessionInfo sessionInfo)
    {
        var expiresAt = _datetimeProvider.UtcNow + TimeSpan.FromSeconds(_refreshTokenValiditySeconds);
        var refreshToken = GenerateRefreshToken();
        var session = new SessionInfo(sessionInfo.Id, sessionInfo.ClientId, sessionInfo.UserId, refreshToken, sessionInfo.Device, sessionInfo.Grant,
            sessionInfo.CreatedAt, _datetimeProvider.UtcNow, expiresAt);

        await _sessionInfoRepository.ReplaceAsync(sessionInfo.Id, session);

        return session;
    }

    private string GenerateRefreshToken()
    {
        var token = "";
        for (var i = 0; i < RefreshTokenLength; i++)
            token += RefreshTokenChars[_random.Next(RefreshTokenChars.Length)];

        return token;
    }

    private async Task<User> AuthenticateUserAsync(PasswordGrantInput passwordGrantInput,
        CancellationToken cancellationToken = new())
    {
        var user = await _userRepository.GetByUsernameAsync(passwordGrantInput.Username, cancellationToken);
        if (!_passwordEncoder.VerifyPassword(user.Password, passwordGrantInput.Password))
            throw new AuthenticationException();

        return user;
    }

    private async Task<Client> AuthenticateClientAsync(ClientCredentials clientCredentials,
        GrantType grantType,
        CancellationToken cancellationToken = new())
    {
        var client = await _clientRepository.GetByClientIdAsync(clientCredentials.ClientId, cancellationToken);
        if (!_passwordEncoder.VerifyPassword(client.ClientSecret, clientCredentials.ClientSecret ?? ""))
            throw new AuthenticationException();

        var role = await _clientRoleRepository.FindByKey(grantType.RoleKey, cancellationToken);
        if (role == null)
            throw new AuthenticationException();

        if (!client.ClientRoles.Contains(role.Id))
            throw new AuthenticationException();

        return client;
    }
}