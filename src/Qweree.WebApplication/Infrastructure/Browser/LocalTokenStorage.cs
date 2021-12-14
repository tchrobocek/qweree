using System.Threading;
using System.Threading.Tasks;
using Qweree.Authentication.Sdk.OAuth2;
using Qweree.Utils;

namespace Qweree.WebApplication.Infrastructure.Browser
{
    public class LocalUserStorage
    {
        private readonly LocalStorage _localStorage;

        public LocalUserStorage(LocalStorage localStorage)
        {
            _localStorage = localStorage;
        }

        public async Task SetUserAsync(UserDto user, CancellationToken cancellationToken = new())
        {
            await _localStorage.SetItemAsync("user", JsonUtils.Serialize(user), cancellationToken);
        }

        public async Task<UserDto?> GetUserAsync(CancellationToken cancellationToken = new())
        {
            var item = await _localStorage.GetItemAsync("user", cancellationToken);
            if (string.IsNullOrEmpty(item))
                return null;

            return JsonUtils.Deserialize<UserDto>(item);
        }

        public async Task RemoveUserAsync(CancellationToken cancellationToken = new())
        {
            await _localStorage.RemoveItem("user", cancellationToken);
        }
    }
}