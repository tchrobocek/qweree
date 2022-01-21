using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace Qweree.WebApplication.Infrastructure.Browser;

public class UnauthorizedHttpHandler : DelegatingHandler
{
    private readonly LocalUserStorage _localUserStorage;
    private readonly NavigationManager _navigationManager;

    public UnauthorizedHttpHandler(LocalUserStorage localUserStorage, NavigationManager navigationManager, HttpMessageHandler innerHandler)
        :base(innerHandler)
    {
        _localUserStorage = localUserStorage;
        _navigationManager = navigationManager;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode != HttpStatusCode.Unauthorized)
            return response;

        await _localUserStorage.RemoveUserAsync(cancellationToken);
        _navigationManager.NavigateTo("/", true);

        return response;
    }
}