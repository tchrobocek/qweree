using System.Threading;
using System.Threading.Tasks;
using Qweree.Sdk.Http.HttpClient;

namespace Qweree.WebApplication.Infrastructure.Browser
{
    public class LocalTokenStorage : ITokenStorage
    {
        private readonly LocalStorage _localStorage;

        public LocalTokenStorage(LocalStorage localStorage)
        {
            _localStorage = localStorage;
        }

        public async Task SetAccessTokenAsync(string token, CancellationToken cancellationToken = new())
        {
            await _localStorage.SetItemAsync("access_token", token, cancellationToken);
        }


        public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = new())
        {
            var storedToken = await _localStorage.GetItemAsync("access_token", cancellationToken);
            return storedToken ?? string.Empty;
        }

        public async Task SetRefreshTokenAsync(string token, CancellationToken cancellationToken = new())
        {
            await _localStorage.SetItemAsync("refresh_token", token, cancellationToken);
        }

        public async Task<string> GetRefreshTokenAsync(CancellationToken cancellationToken = new())
        {
            var storedToken = await _localStorage.GetItemAsync("refresh_token", cancellationToken);
            return storedToken ?? string.Empty;
        }
    }
}