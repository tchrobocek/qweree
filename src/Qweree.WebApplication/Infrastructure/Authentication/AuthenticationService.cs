using System.Threading;
using System.Threading.Tasks;
using Qweree.Authentication.Sdk.OAuth2;
using Qweree.WebApplication.Infrastructure.Browser;

namespace Qweree.WebApplication.Infrastructure.Authentication
{
    public class AuthenticationService
    {
        private readonly OAuth2Adapter _oauthAdapter;
        private readonly LocalStorage _localStorage;

        public AuthenticationService(OAuth2Adapter oauthAdapter, LocalStorage localStorage)
        {
            _oauthAdapter = oauthAdapter;
            _localStorage = localStorage;
        }

        public async Task AuthenticateAsync(string username, string password, CancellationToken cancellationToken = new())
        {
            var tokenInfo = await _oauthAdapter.SignInAsync(new PasswordGrantInput(username, password),
                new ClientCredentials("admin-cli", "password"), cancellationToken);

            await _localStorage.SetItemAsync("access_token", tokenInfo.AccessToken, cancellationToken);
        }

        public async Task LogoutAsync(CancellationToken cancellationToken = new())
        {
            await _localStorage.RemoveItem("access_token", cancellationToken);
        }
    }
}