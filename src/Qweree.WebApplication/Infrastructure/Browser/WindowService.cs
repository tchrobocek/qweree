using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Qweree.WebApplication.Infrastructure.Browser;

public class WindowService
{
    private readonly IJSRuntime _runtime;

    public WindowService(IJSRuntime runtime)
    {
        _runtime = runtime;
    }

    public async Task OpenInNewTabAsync(Uri uri, CancellationToken cancellationToken = new())
    {
        await _runtime.InvokeVoidAsync("window.open", cancellationToken, uri, "_blank");
    }
}