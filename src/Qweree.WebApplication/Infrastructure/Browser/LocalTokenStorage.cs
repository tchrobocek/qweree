using System.Threading;
using System.Threading.Tasks;
using Qweree.Authentication.Sdk.Session;
using Qweree.Utils;

namespace Qweree.WebApplication.Infrastructure.Browser;

public class LocalUserStorage
{
    private readonly LocalStorage _localStorage;

    public LocalUserStorage(LocalStorage localStorage)
    {
        _localStorage = localStorage;
    }

    public async Task SetUserAsync(Identity identity, CancellationToken cancellationToken = new())
    {
        await _localStorage.SetItemAsync("identity", JsonUtils.Serialize(identity), cancellationToken);
    }

    public async Task<Identity?> GetIdentityAsync(CancellationToken cancellationToken = new())
    {
        var item = await _localStorage.GetItemAsync("identity", cancellationToken);
        if (string.IsNullOrEmpty(item))
            return null;

        return JsonUtils.Deserialize<Identity>(item);
    }

    public async Task RemoveUserAsync(CancellationToken cancellationToken = new())
    {
        await _localStorage.RemoveItem("identity", cancellationToken);
    }
}