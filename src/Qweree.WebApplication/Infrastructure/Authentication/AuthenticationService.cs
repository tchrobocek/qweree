using System;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Authentication.Sdk.OAuth2;
using Qweree.Utils;
using Qweree.WebApplication.Infrastructure.Browser;

namespace Qweree.WebApplication.Infrastructure.Authentication
{
    public class AuthenticationService
    {
        private readonly OAuth2Client _oauthClient;
        private readonly LocalTokenStorage _localTokenStorage;

        public AuthenticationService(OAuth2Client oauthClient, LocalTokenStorage localTokenStorage)
        {
            _oauthClient = oauthClient;
            _localTokenStorage = localTokenStorage;
        }

        public async Task AuthenticateAsync(string username, string password, CancellationToken cancellationToken = new())
        {
            var response = await _oauthClient.SignInAsync(new PasswordGrantInput(username, password),
                new ClientCredentials("admin-cli", "password"), cancellationToken);

            response.EnsureSuccessStatusCode();

            var tokenInfo = await response.ReadPayloadAsync(JsonUtils.SnakeCaseNamingPolicy, cancellationToken);
            await _localTokenStorage.SetAccessTokenAsync(tokenInfo?.AccessToken!, cancellationToken);
            await _localTokenStorage.SetRefreshTokenAsync(tokenInfo?.RefreshToken!, cancellationToken);
        }

        public async Task RefreshAsync(CancellationToken cancellationToken = new())
        {
            var refreshToken = await _localTokenStorage.GetRefreshTokenAsync(cancellationToken);

            if (string.IsNullOrEmpty(refreshToken))
                throw new ArgumentException("There is no refresh token stored.");

            var response = await _oauthClient.RefreshAsync(new RefreshTokenGrantInput(refreshToken),
                new ClientCredentials("admin-cli", "password"), cancellationToken);

            response.EnsureSuccessStatusCode();

            var tokenInfo = await response.ReadPayloadAsync(JsonUtils.SnakeCaseNamingPolicy, cancellationToken);
            await _localTokenStorage.SetAccessTokenAsync(tokenInfo?.AccessToken!, cancellationToken);
        }

        public async Task LogoutAsync(CancellationToken cancellationToken = new())
        {
            await _localTokenStorage.SetAccessTokenAsync("", cancellationToken);
        }
    }
}