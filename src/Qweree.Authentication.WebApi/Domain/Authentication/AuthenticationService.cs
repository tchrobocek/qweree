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
using Qweree.AspNet.Session;
using Qweree.Authentication.Sdk.Tokens;
using Qweree.Authentication.WebApi.Domain.Identity;
using Qweree.Authentication.WebApi.Domain.Security;
using Qweree.Utils;
using User = Qweree.Authentication.WebApi.Domain.Identity.User;

namespace Qweree.Authentication.WebApi.Domain.Authentication
{
    public class AuthenticationService
    {
        public const string Audience = "qweree";
        public const string Issuer = "net.qweree";
        private const string AccessDeniedMessage = "Access denied.";
        private const int RefreshTokenLength = 16;
        private readonly string _accessTokenKey;

        private readonly int _accessTokenValiditySeconds;
        private readonly IDateTimeProvider _datetimeProvider;
        private readonly string _fileAccessTokenKey;
        private readonly int _fileAccessTokenValiditySeconds;
        private readonly Random _random;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly int _refreshTokenValiditySeconds;
        private readonly IUserRepository _userRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IPasswordEncoder _passwordEncoder;

        private readonly string RefreshTokenChars = "0123456789abcdefghijklmnopqrstuvwxyz";

        public AuthenticationService(IUserRepository userRepository, IRefreshTokenRepository refreshTokenRepository,
            IDateTimeProvider datetimeProvider, Random random,
            int accessTokenValiditySeconds, int refreshTokenValiditySeconds, string accessTokenKey,
            string fileAccessTokenKey, int fileAccessTokenValiditySeconds, IPasswordEncoder passwordEncoder,
            IClientRepository clientRepository)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _datetimeProvider = datetimeProvider;
            _accessTokenValiditySeconds = accessTokenValiditySeconds;
            _accessTokenKey = accessTokenKey;
            _fileAccessTokenKey = fileAccessTokenKey;
            _fileAccessTokenValiditySeconds = fileAccessTokenValiditySeconds;
            _passwordEncoder = passwordEncoder;
            _clientRepository = clientRepository;
            _refreshTokenValiditySeconds = refreshTokenValiditySeconds;
            _random = random;
        }

        public async Task<Response<TokenInfo>> AuthenticateAsync(PasswordGrantInput input,
            ClientCredentials clientCredentials,
            CancellationToken cancellationToken = new())
        {
            var now = _datetimeProvider.UtcNow;

            User user;
            Client client;

            try
            {
                user = await AuthenticateUserAsync(input, cancellationToken);
                client = await AuthenticateClientAsync(clientCredentials, cancellationToken);
            }
            catch (Exception)
            {
                return Response.Fail<TokenInfo>(AccessDeniedMessage);
            }

            var expiresAt = now + TimeSpan.FromSeconds(_accessTokenValiditySeconds);
            var accessToken = new AccessToken(clientCredentials.ClientId, user.Id, user.Username, user.FullName,
                user.ContactEmail, user.Roles, now,
                expiresAt);
            var jwt = EncodeAccessToken(accessToken);

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
                await AuthenticateClientAsync(clientCredentials, cancellationToken);
            }
            catch (Exception)
            {
                return Response.Fail<TokenInfo>(AccessDeniedMessage);
            }

            if (token.ExpiresAt < now)
                return Response.Fail<TokenInfo>(AccessDeniedMessage);

            var expiresAt = now + TimeSpan.FromSeconds(_accessTokenValiditySeconds);
            var accessToken = new AccessToken(clientCredentials.ClientId, user.Id, user.Username, user.FullName,
                user.ContactEmail, user.Roles, now, expiresAt);
            var jwt = EncodeAccessToken(accessToken);

            var tokenInfo = new TokenInfo(jwt, input.RefreshToken, expiresAt);
            return Response.Ok(tokenInfo);
        }

        public async Task<Response<TokenInfo>> AuthenticateAsync(FileAccessGrantInput input,
            ClientCredentials clientCredentials, CancellationToken cancellationToken = new())
        {
            var validationParameters = Startup.GetValidationParameters(_fileAccessTokenKey);
            var now = _datetimeProvider.UtcNow;

            ClaimsPrincipalStorage session;

            try
            {
                await AuthenticateClientAsync(clientCredentials, cancellationToken);
                var claimsPrincipal =
                    new JwtSecurityTokenHandler().ValidateToken(input.AccessToken, validationParameters, out _);
                session = new ClaimsPrincipalStorage(claimsPrincipal);
            }
            catch (Exception)
            {
                return Response.Fail<TokenInfo>(AccessDeniedMessage);
            }

            var expiresAt = now + TimeSpan.FromSeconds(_fileAccessTokenValiditySeconds);
            var fileAccessToken = EncodeFileAccessToken(new AccessToken(clientCredentials.ClientId,
                session.CurrentUser.Id,
                session.CurrentUser.Username, session.CurrentUser.FullName, session.CurrentUser.Email,
                session.CurrentUser.Roles, now, expiresAt));

            return Response.Ok(new TokenInfo(fileAccessToken, expiresAt));
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

        private string EncodeAccessToken(AccessToken accessToken)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_accessTokenKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new("userId", accessToken.UserId.ToString()),
                new("username", accessToken.Username),
                new("full_name", accessToken.FullName),
                new("email", accessToken.Email),
                new("iat", accessToken.IssuedAt.Ticks.ToString()),
                new("jti", Guid.NewGuid().ToString())
            };
            claims.AddRange(accessToken.Roles.Select(role => new Claim("role", role)));

            var token = new JwtSecurityToken(Issuer, Audience, claims,
                expires: accessToken.ExpiresAt, signingCredentials: credentials, notBefore: _datetimeProvider.UtcNow);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string EncodeFileAccessToken(AccessToken accessToken)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_fileAccessTokenKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new("username", accessToken.Username),
                new("iat", accessToken.IssuedAt.Ticks.ToString()),
                new("jti", Guid.NewGuid().ToString())
            };
            claims.AddRange(accessToken.Roles.Select(role => new Claim("role", role)));

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
            CancellationToken cancellationToken = new())
        {
            var client = await _clientRepository.GetByClientIdAsync(clientCredentials.ClientId, cancellationToken);
            if (!_passwordEncoder.VerifyPassword(client.ClientSecret, clientCredentials.ClientSecret ?? ""))
                throw new AuthenticationException();

            return client;
        }
    }
}