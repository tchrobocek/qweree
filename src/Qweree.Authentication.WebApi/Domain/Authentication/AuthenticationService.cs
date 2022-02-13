using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;
using Qweree.AspNet.Application;
using Qweree.Authentication.Sdk.OAuth2;
using Qweree.Authentication.WebApi.Domain.Authorization;
using Qweree.Authentication.WebApi.Domain.Authorization.Roles;
using Qweree.Authentication.WebApi.Domain.Identity;
using Qweree.Authentication.WebApi.Domain.Security;
using Qweree.Session;
using Qweree.Session.Tokens;
using Qweree.Utils;
using Client = Qweree.Authentication.WebApi.Domain.Identity.Client;
using User = Qweree.Authentication.WebApi.Domain.Identity.User;

namespace Qweree.Authentication.WebApi.Domain.Authentication;

public class AuthenticationService
{
    private const string AccessDeniedMessage = "Access denied.";
    private const int RefreshTokenLength = 16;

    private readonly int _accessTokenValiditySeconds;
    private readonly IDateTimeProvider _datetimeProvider;
    private readonly Random _random;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly int _refreshTokenValiditySeconds;
    private readonly IUserRepository _userRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IPasswordEncoder _passwordEncoder;
    private readonly AuthorizationService _authorizationService;
    private readonly IClientRoleRepository _clientRoleRepository;
    private readonly ITokenEncoder _tokenEncoder;

    private readonly string RefreshTokenChars = "0123456789abcdefghijklmnopqrstuvwxyz";

    public AuthenticationService(IUserRepository userRepository, IRefreshTokenRepository refreshTokenRepository,
        IDateTimeProvider datetimeProvider, Random random,
        int accessTokenValiditySeconds, int refreshTokenValiditySeconds, IPasswordEncoder passwordEncoder,
        IClientRepository clientRepository, AuthorizationService authorizationService, IClientRoleRepository clientRoleRepository, ITokenEncoder tokenEncoder)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _datetimeProvider = datetimeProvider;
        _accessTokenValiditySeconds = accessTokenValiditySeconds;
        _passwordEncoder = passwordEncoder;
        _clientRepository = clientRepository;
        _authorizationService = authorizationService;
        _clientRoleRepository = clientRoleRepository;
        _tokenEncoder = tokenEncoder;
        _refreshTokenValiditySeconds = refreshTokenValiditySeconds;
        _random = random;
    }

    public async Task<Response<TokenInfo>> AuthenticateAsync(PasswordGrantInput input,
        ClientCredentials clientCredentials, CancellationToken cancellationToken = new())
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

        var effectiveRoles = new List<string>();
        await foreach (var role in _authorizationService.GetEffectiveUserRoles(user, cancellationToken)
                           .WithCancellation(cancellationToken))
        {
            effectiveRoles.Add(role.Key);
        }

        effectiveRoles.Add("USER");

        var expiresAt = now + TimeSpan.FromSeconds(_accessTokenValiditySeconds);
        var identity = new Session.Identity(new IdentityClient(client.Id, client.ClientId, client.ApplicationName),
            new IdentityUser(user.Id, user.Username, user.FullName),
            user.ContactEmail, effectiveRoles.ToImmutableArray());
        var accessToken = new AccessToken(identity, now, expiresAt);
        var jwt = _tokenEncoder.EncodeAccessToken(accessToken);

        var refreshToken = await GenerateRefreshTokenAsync(user, client, cancellationToken);

        var tokenInfo = new TokenInfo(jwt, refreshToken, expiresAt);
        return Response.Ok(tokenInfo);
    }

    public async Task<Response<TokenInfo>> AuthenticateAsync(RefreshTokenGrantInput input,
        ClientCredentials clientCredentials, CancellationToken cancellationToken = new())
    {
        var now = _datetimeProvider.UtcNow;

        RefreshToken token;
        User user;
        Client client;

        try
        {
            token = await _refreshTokenRepository.GetByTokenAsync(input.RefreshToken, cancellationToken);
            user = await _userRepository.GetAsync(token.UserId, cancellationToken);
            client = await AuthenticateClientAsync(clientCredentials, GrantType.RefreshToken, cancellationToken);
        }
        catch (Exception)
        {
            return Response.Fail<TokenInfo>(AccessDeniedMessage);
        }

        if (token.ExpiresAt < now)
            return Response.Fail<TokenInfo>(AccessDeniedMessage);

        var effectiveRoles = new List<string>();
        await foreach (var role in _authorizationService.GetEffectiveUserRoles(user, cancellationToken)
                           .WithCancellation(cancellationToken))
        {
            effectiveRoles.Add(role.Key);
        }

        effectiveRoles.Add("USER");

        var expiresAt = now + TimeSpan.FromSeconds(_accessTokenValiditySeconds);
        var identity = new Session.Identity(new IdentityClient(client.Id, client.ClientId, client.ApplicationName),
            new IdentityUser(user.Id, user.Username, user.FullName),
            user.ContactEmail, effectiveRoles.ToImmutableArray());
        var accessToken = new AccessToken(identity, now, expiresAt);
        var jwt = _tokenEncoder.EncodeAccessToken(accessToken);

        var tokenInfo = new TokenInfo(jwt, input.RefreshToken, expiresAt);
        return Response.Ok(tokenInfo);
    }

    public async Task<Response<TokenInfo>> AuthenticateAsync(ClientCredentials clientCredentials, CancellationToken cancellationToken = new())
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

        var effectiveRoles = new List<string>();
        await foreach (var role in _authorizationService.GetEffectiveUserRoles(client, cancellationToken)
                           .WithCancellation(cancellationToken))
        {
            effectiveRoles.Add(role.Key);
        }

        effectiveRoles.Add("CLIENT");

        var expiresAt = now + TimeSpan.FromSeconds(_accessTokenValiditySeconds);
        var identity = new Session.Identity(new IdentityClient(client.Id, client.ClientId, client.ApplicationName),
            owner.ContactEmail, effectiveRoles.ToImmutableArray());
        var accessToken = new AccessToken(identity, now, expiresAt);
        var jwt = _tokenEncoder.EncodeAccessToken(accessToken);

        var tokenInfo = new TokenInfo(jwt, null, expiresAt);
        return Response.Ok(tokenInfo);
    }

    private async Task<string?> GenerateRefreshTokenAsync(User user, Client client,
        CancellationToken cancellationToken = new())
    {
        var token = "";
        for (var i = 0; i < RefreshTokenLength; i++)
            token += RefreshTokenChars[_random.Next(RefreshTokenChars.Length)];

        var expiresAt = _datetimeProvider.UtcNow + TimeSpan.FromSeconds(_refreshTokenValiditySeconds);
        var refreshToken = new RefreshToken(Guid.NewGuid(), token, client.Id, user.Id, expiresAt, _datetimeProvider.UtcNow);

        await _refreshTokenRepository.InsertAsync(refreshToken, cancellationToken);

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