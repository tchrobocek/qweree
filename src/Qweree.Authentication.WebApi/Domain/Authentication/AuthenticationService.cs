using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using Qweree.AspNet.Application;
using Qweree.Authentication.Sdk.Session;
using Qweree.Authentication.Sdk.Session.Tokens;
using Qweree.Authentication.WebApi.Domain.Authorization;
using Qweree.Authentication.WebApi.Domain.Identity;
using Qweree.Authentication.WebApi.Domain.Security;
using Qweree.Authentication.WebApi.Domain.Session;
using Qweree.Utils;
using Client = Qweree.Authentication.WebApi.Domain.Identity.Client;
using IdentityClient = Qweree.Authentication.WebApi.Domain.Session.IdentityClient;
using IdentityMapper = Qweree.Authentication.WebApi.Infrastructure.Session.IdentityMapper;
using IdentityUser = Qweree.Authentication.WebApi.Domain.Session.IdentityUser;
using User = Qweree.Authentication.WebApi.Domain.Identity.User;
using SdkAccessToken = Qweree.Authentication.Sdk.Session.Tokens.AccessToken;

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
    private readonly ITokenEncoder _tokenEncoder;
    private readonly ISessionInfoRepository _sessionInfoRepository;
    private readonly ISessionStorage _sessionStorage;
    private readonly RSA _rsa;

    private const string RefreshTokenChars = "0123456789abcdefghijklmnopqrstuvwxyz";

    public AuthenticationService(IUserRepository userRepository,
        IDateTimeProvider datetimeProvider, Random random, int accessTokenValiditySeconds, int refreshTokenValiditySeconds, IPasswordEncoder passwordEncoder,
        IClientRepository clientRepository, AuthorizationService authorizationService,
        ITokenEncoder tokenEncoder, ISessionInfoRepository sessionInfoRepository, ISessionStorage sessionStorage, RSA rsa)
    {
        _userRepository = userRepository;
        _datetimeProvider = datetimeProvider;
        _accessTokenValiditySeconds = accessTokenValiditySeconds;
        _passwordEncoder = passwordEncoder;
        _clientRepository = clientRepository;
        _authorizationService = authorizationService;
        _tokenEncoder = tokenEncoder;
        _sessionInfoRepository = sessionInfoRepository;
        _sessionStorage = sessionStorage;
        _rsa = rsa;
        _refreshTokenValiditySeconds = refreshTokenValiditySeconds;
        _random = random;
    }

    public async Task<Response<TokenInfo>> AuthenticateAsync(PasswordGrantInput input,
        ClientCredentials clientCredentials, string ipAddress, UserAgentInfo? userAgent, CancellationToken cancellationToken = new())
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

        var session = await BeginSessionAsync(client, user, ipAddress, userAgent, GrantType.Password, true);

        var effectiveRoles = new List<string>();
        await foreach (var role in _authorizationService.GetEffectiveRoles(user.Roles, cancellationToken)
                           .WithCancellation(cancellationToken))
        {
            effectiveRoles.Add(role.Key);
        }

        var expiresAt = now + TimeSpan.FromSeconds(_accessTokenValiditySeconds);
        var identity = new Session.Identity(new(client.Id, client.ClientId, client.ApplicationName),
            new IdentityUser(user.Id, user.Username, user.Properties.Select(p => new UserProperty(p.Key, p.Value)).ToImmutableArray()),
            user.ContactEmail, effectiveRoles.ToImmutableArray());
        var jwt = _tokenEncoder.EncodeAccessToken(new SdkAccessToken
        {
            ExpiresAt = expiresAt,
            Identity = IdentityMapper.Map(identity),
            IssuedAt = _datetimeProvider.UtcNow,
            SessionId = session.Id
        }, new RsaSecurityKey(_rsa));

        var expiresIn = expiresAt - _datetimeProvider.UtcNow;
        var tokenInfo = new TokenInfo(jwt, session.RefreshToken, (int)expiresIn.TotalSeconds);
        return Response.Ok(tokenInfo);
    }

    public async Task<Response<TokenInfo>> AuthenticateAsync(RefreshTokenGrantInput input,
        ClientCredentials clientCredentials, string ipAddress, UserAgentInfo? userAgent, CancellationToken cancellationToken = new())
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

        var session = await RefreshSessionAsync(sessionInfo, ipAddress, userAgent);

        if (session.ExpiresAt < now)
            return Response.Fail<TokenInfo>(AccessDeniedMessage);

        var effectiveRoles = new List<string>();
        await foreach (var role in _authorizationService.GetEffectiveRoles(user.Roles, cancellationToken)
                           .WithCancellation(cancellationToken))
        {
            effectiveRoles.Add(role.Key);
        }

        var expiresAt = now + TimeSpan.FromSeconds(_accessTokenValiditySeconds);
        var identity = new Session.Identity(new IdentityClient(client.Id, client.ClientId, client.ApplicationName),
            new IdentityUser(user.Id, user.Username,
                user.Properties.Select(p => new UserProperty(p.Key, p.Value)).ToImmutableArray()),
            user.ContactEmail, effectiveRoles.ToImmutableArray());
        var jwt = _tokenEncoder.EncodeAccessToken(new SdkAccessToken
        {
            ExpiresAt = expiresAt,
            Identity = IdentityMapper.Map(identity),
            IssuedAt = _datetimeProvider.UtcNow,
            SessionId = session.Id
        }, new RsaSecurityKey(_rsa));

        var expiresIn = expiresAt - _datetimeProvider.UtcNow;
        var tokenInfo = new TokenInfo(jwt, session.RefreshToken, (int)expiresIn.TotalSeconds);
        return Response.Ok(tokenInfo);
    }

    public async Task<Response<TokenInfo>> AuthenticateAsync(ClientCredentials clientCredentials, string ipAddress, UserAgentInfo? userAgent, CancellationToken cancellationToken = new())
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

        var session = await BeginSessionAsync(client, null, ipAddress, userAgent, GrantType.ClientCredentials, false);

        var definition = client.AccessDefinitions.OfType<ClientCredentialsAccessDefinition>()
            .Single();

        var effectiveRoles = new List<string>();
        await foreach (var role in _authorizationService.GetEffectiveRoles(definition.Roles, cancellationToken)
                           .WithCancellation(cancellationToken))
        {
            effectiveRoles.Add(role.Key);
        }

        var expiresAt = now + TimeSpan.FromSeconds(_accessTokenValiditySeconds);
        var identity = new Session.Identity(new IdentityClient(client.Id, client.ClientId, client.ApplicationName),
            owner.ContactEmail, effectiveRoles.ToImmutableArray());
        var jwt = _tokenEncoder.EncodeAccessToken(new SdkAccessToken
        {
            ExpiresAt = expiresAt,
            Identity = IdentityMapper.Map(identity),
            IssuedAt = _datetimeProvider.UtcNow,
            SessionId = session.Id
        }, new RsaSecurityKey(_rsa));

        var expiresIn = expiresAt - _datetimeProvider.UtcNow;
        var tokenInfo = new TokenInfo(jwt, null, (int)expiresIn.TotalSeconds);
        return Response.Ok(tokenInfo);
    }

    public async Task<Response<TokenInfo>> AuthenticateAsync(ImplicitGrantInput grantInput, string ipAddress, UserAgentInfo? userAgent, CancellationToken cancellationToken = new())
    {
        if (_sessionStorage.IsAnonymous)
            return Response.Fail<TokenInfo>(AccessDeniedMessage);

        var now = _datetimeProvider.UtcNow;

        User owner;
        User user;
        Client client;

        try
        {
            client = await AuthenticateClientAsync(grantInput, cancellationToken);
            owner = await _userRepository.GetAsync(client.OwnerId, cancellationToken);
            user = await _userRepository.GetAsync(_sessionStorage.UserId, cancellationToken);
        }
        catch (Exception)
        {
            return Response.Fail<TokenInfo>(AccessDeniedMessage);
        }

        var session = await BeginSessionAsync(client, user, ipAddress, userAgent, GrantType.Implicit, false);

        var effectiveRoles = new List<string>();
        await foreach (var role in _authorizationService.GetEffectiveRoles(user.Roles, cancellationToken)
                           .WithCancellation(cancellationToken))
        {
            effectiveRoles.Add(role.Key);
        }

        var expiresAt = now + TimeSpan.FromSeconds(_accessTokenValiditySeconds);
        var identity = new Session.Identity(new IdentityClient(client.Id, client.ClientId, client.ApplicationName),
            owner.ContactEmail, effectiveRoles.ToImmutableArray());
        var jwt = _tokenEncoder.EncodeAccessToken(new SdkAccessToken
        {
            ExpiresAt = expiresAt,
            Identity = IdentityMapper.Map(identity),
            IssuedAt = _datetimeProvider.UtcNow,
            SessionId = session.Id
        }, new RsaSecurityKey(_rsa));

        var expiresIn = expiresAt - _datetimeProvider.UtcNow;
        var tokenInfo = new TokenInfo(jwt, null, (int)expiresIn.TotalSeconds);
        return Response.Ok(tokenInfo);
    }

    private async Task<SessionInfo> BeginSessionAsync(Client client, User? user, string ipAddress, UserAgentInfo? userAgent, GrantType grantType, bool issueRefreshToken)
    {
        var expiresAt  = _datetimeProvider.UtcNow + TimeSpan.FromSeconds(_accessTokenValiditySeconds);
        var refreshToken = string.Empty;

        if (issueRefreshToken)
        {
            expiresAt = _datetimeProvider.UtcNow + TimeSpan.FromSeconds(_refreshTokenValiditySeconds);
            refreshToken = GenerateRefreshToken();
        }

        var session = new SessionInfo(Guid.NewGuid(), client.Id, user?.Id, refreshToken, ipAddress, userAgent, grantType,
            _datetimeProvider.UtcNow, _datetimeProvider.UtcNow, expiresAt);

        await _sessionInfoRepository.InsertAsync(session);
        
        return session;
    }

    private async Task<SessionInfo> RefreshSessionAsync(SessionInfo sessionInfo, string ipAddress, UserAgentInfo? userAgentInfo)
    {
        var expiresAt = _datetimeProvider.UtcNow + TimeSpan.FromSeconds(_refreshTokenValiditySeconds);
        var refreshToken = GenerateRefreshToken();
        var session = new SessionInfo(sessionInfo.Id, sessionInfo.ClientId, sessionInfo.UserId, refreshToken, ipAddress, userAgentInfo, sessionInfo.Grant,
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
        GrantType grantType, CancellationToken cancellationToken = new())
    {
        var client = await _clientRepository.GetByClientIdAsync(clientCredentials.ClientId, cancellationToken);
        if (!_passwordEncoder.VerifyPassword(client.ClientSecret, clientCredentials.ClientSecret ?? ""))
            throw new AuthenticationException();

        var key = grantType.Key;
        if (key == GrantType.RefreshToken.Key)
            key = GrantType.Password.Key;

        var accessDefinition = client.AccessDefinitions.FirstOrDefault(d => d.GrantType.Key == key);
        if (accessDefinition is null)
            throw new AuthenticationException();

        return client;
    }

    private async Task<Client> AuthenticateClientAsync(ImplicitGrantInput grantInput,
        CancellationToken cancellationToken = new())
    {
        var client = await _clientRepository.GetByClientIdAsync(grantInput.ClientId, cancellationToken);

        var key = GrantType.Implicit.Key;
        var accessDefinitions = client.AccessDefinitions.Where(d => d.GrantType.Key == key)
            .ToArray();
        if (!accessDefinitions.Cast<ImplicitAccessDefinition>().Any(a => a.RedirectUri == grantInput.RedirectUri))
            throw new AuthenticationException();

        return client;
    }

    public async Task<Response> RevokeAsync()
    {
        if (_sessionStorage.IsAnonymous)
            return Response.Fail(AccessDeniedMessage);

        var session = _sessionStorage.SessionId;
        await _sessionInfoRepository.DeleteOneAsync(session);

        return Response.Ok();
    }
}