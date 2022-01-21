using System.Threading;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Qweree.WebApplication.Infrastructure.Browser;

public class LocalStorage
{
    private readonly IJSRuntime _jsRuntime;

    public LocalStorage(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task SetItemAsync(string key, string value, CancellationToken cancellationToken = new())
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", cancellationToken, key, value);
    }

    public async Task<string?> GetItemAsync(string key, CancellationToken cancellationToken = new())
    {
        return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", cancellationToken, key);
    }

    public async Task RemoveItem(string key, CancellationToken cancellationToken = new())
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", cancellationToken, key);
    }

    public async Task ClearStorage(CancellationToken cancellationToken = new())
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.clear", cancellationToken);
    }
}