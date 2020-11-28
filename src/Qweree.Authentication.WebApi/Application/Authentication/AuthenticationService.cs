using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using Qweree.AspNet.Application;
using Qweree.Authentication.Sdk;
using Qweree.Authentication.Sdk.Authentication;
using Qweree.Authentication.WebApi.Domain.Authentication;
using Qweree.Authentication.WebApi.Domain.Identity;
using Qweree.Utils;

namespace Qweree.Authentication.WebApi.Application.Authentication
{
    public class AuthenticationService
    {
        public const string Audience = "localhost";
        public const string Issuer = "net.qweree";
        private const string AccessDeniedMessage = "Access denied.";
        private const int RefreshTokenLength = 16;

        private readonly string RefreshTokenChars = "0123456789abcdefghijklmnopqrstuvwxyz";

        private readonly int _accessTokenValiditySeconds;
        private readonly int _refreshTokenValiditySeconds;
        private readonly string _accessTokenKey;
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IDateTimeProvider _datetimeProvider;
        private readonly Random _random;

        public AuthenticationService(IUserRepository userRepository, IRefreshTokenRepository refreshTokenRepository, IDateTimeProvider datetimeProvider, Random random,
            int accessTokenValiditySeconds, int refreshTokenValiditySeconds, string accessTokenKey)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _datetimeProvider = datetimeProvider;
            _accessTokenValiditySeconds = accessTokenValiditySeconds;
            _accessTokenKey = accessTokenKey;
            _refreshTokenValiditySeconds = refreshTokenValiditySeconds;
            _random = random;
        }

        public async Task<Response<TokenInfo>> AuthenticateAsync(PasswordGrantInput input, CancellationToken cancellationToken = new CancellationToken())
        {
            var now = _datetimeProvider.UtcNow;

            User user;

            try
            {
                user = await _userRepository.GetByUsernameAsync(input.Username, cancellationToken);
            }
            catch (Exception)
            {
                return Response.Fail<TokenInfo>(AccessDeniedMessage);
            }

            if (!ValidatePassword(input.Password, user.Password))
            {
                return Response.Fail<TokenInfo>(AccessDeniedMessage);
            }

            var expiresAt = now + TimeSpan.FromSeconds(_accessTokenValiditySeconds);
            var accessToken = new AccessToken(user.Username, user.Roles, now, expiresAt);
            var jwt = EncodeAccessToken(accessToken);

            var refreshToken = await GenerateRefreshTokenAsync(user, cancellationToken);

            var tokenInfo = new TokenInfo(jwt, refreshToken, expiresAt);
            return Response.Ok(tokenInfo);
        }

        public async Task<Response<TokenInfo>> AuthenticateAsync(RefreshTokenGrantInput input, CancellationToken cancellationToken = new CancellationToken())
        {
            var now = _datetimeProvider.UtcNow;

            RefreshToken token;
            User user;

            try
            {
                token = await _refreshTokenRepository.GetByTokenAsync(input.RefreshToken, cancellationToken);
                user = await _userRepository.GetAsync(token.UserId, cancellationToken);
            }
            catch (Exception)
            {
                return Response.Fail<TokenInfo>(AccessDeniedMessage);
            }

            if (token.ExpiresAt < now)
            {
                return Response.Fail<TokenInfo>(AccessDeniedMessage);
            }

            var expiresAt = now + TimeSpan.FromSeconds(_accessTokenValiditySeconds);
            var accessToken = new AccessToken(user.Username, user.Roles, now, expiresAt);
            var jwt = EncodeAccessToken(accessToken);

            var tokenInfo = new TokenInfo(jwt, input.RefreshToken, expiresAt);
            return Response.Ok(tokenInfo);
        }

        private async Task<string?> GenerateRefreshTokenAsync(User user, CancellationToken cancellationToken = new CancellationToken())
        {
            var token = "";
            for (var i = 0; i < RefreshTokenLength; i++)
            {
                token += RefreshTokenChars[_random.Next(RefreshTokenChars.Length)];
            }

            var expiresAt = _datetimeProvider.UtcNow + TimeSpan.FromSeconds(_refreshTokenValiditySeconds);
            var refreshToken = new RefreshToken(Guid.NewGuid(), token, user.Id, expiresAt, _datetimeProvider.UtcNow);

            await _refreshTokenRepository.InsertAsync(refreshToken, cancellationToken);

            return token;
        }

        private string EncodeAccessToken(AccessToken accessToken)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_accessTokenKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new Claim("username", accessToken.Username),
                new Claim("iat", accessToken.IssuedAt.Ticks.ToString()),
                new Claim("jti", Guid.NewGuid().ToString())
            };
            claims.AddRange(accessToken.Roles.Select(role => new Claim("role", role)));

            var token = new JwtSecurityToken(Issuer, Audience, claims,
                expires: accessToken.ExpiresAt, signingCredentials: credentials, notBefore: _datetimeProvider.UtcNow);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private bool ValidatePassword(string password, string hashed)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashed);
        }
    }
}