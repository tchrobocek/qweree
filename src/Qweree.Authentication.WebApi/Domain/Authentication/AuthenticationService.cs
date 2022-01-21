using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using Qweree.AspNet.Application;
using Qweree.Authentication.Sdk.Tokens;
using Qweree.Authentication.WebApi.Domain.Authorization;
using Qweree.Authentication.WebApi.Domain.Authorization.Roles;
using Qweree.Authentication.WebApi.Domain.Identity;
using Qweree.Authentication.WebApi.Domain.Security;
using Qweree.Utils;
using User = Qweree.Authentication.WebApi.Domain.Identity.User;

namespace Qweree.Authentication.WebApi.Domain.Authentication;

public class AuthenticationService
{
    public const string Audience = "qweree";
    public const string Issuer = "net.qweree";
    private const string AccessDeniedMessage = "Access denied.";
    private const int RefreshTokenLength = 16;
    private readonly string _accessTokenKey;

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

    private readonly string RefreshTokenChars = "0123456789abcdefghijklmnopqrstuvwxyz";

    public AuthenticationService(IUserRepository userRepository, IRefreshTokenRepository refreshTokenRepository,
        IDateTimeProvider datetimeProvider, Random random,
        int accessTokenValiditySeconds, int refreshTokenValiditySeconds, string accessTokenKey, IPasswordEncoder passwordEncoder,
        IClientRepository clientRepository, AuthorizationService authorizationService, IClientRoleRepository clientRoleRepository)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _datetimeProvider = datetimeProvider;
        _accessTokenValiditySeconds = accessTokenValiditySeconds;
        _accessTokenKey = accessTokenKey;
        _passwordEncoder = passwordEncoder;
        _clientRepository = clientRepository;
        _authorizationService = authorizationService;
        _clientRoleRepository = clientRoleRepository;
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

        var expiresAt = now + TimeSpan.FromSeconds(_accessTokenValiditySeconds);
        var accessToken = new AccessToken(clientCredentials.ClientId, user.Id, user.Username, user.FullName,
            user.ContactEmail, effectiveRoles, now, expiresAt);
        var jwt = EncodeAccessToken(accessToken, true);

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

        try
        {
            token = await _refreshTokenRepository.GetByTokenAsync(input.RefreshToken, cancellationToken);
            user = await _userRepository.GetAsync(token.UserId, cancellationToken);
            await AuthenticateClientAsync(clientCredentials, GrantType.RefreshToken, cancellationToken);
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

        var expiresAt = now + TimeSpan.FromSeconds(_accessTokenValiditySeconds);
        var accessToken = new AccessToken(clientCredentials.ClientId, user.Id, user.Username, user.FullName,
            user.ContactEmail, effectiveRoles, now, expiresAt);
        var jwt = EncodeAccessToken(accessToken, true);

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


        var expiresAt = now + TimeSpan.FromSeconds(_accessTokenValiditySeconds);
        var accessToken = new AccessToken(clientCredentials.ClientId, owner.ContactEmail, effectiveRoles, now, expiresAt);
        var jwt = EncodeAccessToken(accessToken, false);

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

    private string EncodeAccessToken(AccessToken accessToken, bool isUser)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_accessTokenKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new List<Claim>
        {
            new("user_id", accessToken.UserId?.ToString() ?? string.Empty),
            new("clientId", accessToken.ClientId),
            new("username", accessToken.Username ?? string.Empty),
            new("full_name", accessToken.FullName ?? string.Empty),
            new("email", accessToken.Email),
            new("iat", accessToken.IssuedAt.Ticks.ToString()),
            new("jti", Guid.NewGuid().ToString())
        };

        claims.AddRange(accessToken.Roles?.Select(role => new Claim("role", role)) ?? Array.Empty<Claim>());

        if (isUser)
            claims.Add(new Claim("role", "USER"));
        else
            claims.Add(new Claim("role", "CLIENT"));

        var token = new JwtSecurityToken(Issuer, Audience, claims,
            expires: accessToken.ExpiresAt, signingCredentials: credentials, notBefore: _datetimeProvider.UtcNow);

        return new JwtSecurityTokenHandler().WriteToken(token);
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