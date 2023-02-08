using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Qweree.WebApplication.Infrastructure.Authentication;

namespace Qweree.WebApplication.Infrastructure.Browser;

public class UnauthorizedHttpHandler : DelegatingHandler
{
    private readonly AuthenticationService _authenticationService;
    private readonly NavigationManager _navigationManager;

    public UnauthorizedHttpHandler(AuthenticationService authenticationService, NavigationManager navigationManager, HttpMessageHandler innerHandler)
        :base(innerHandler)
    {
        _authenticationService = authenticationService;
        _navigationManager = navigationManager;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);
        //
        // if (response.StatusCode != HttpStatusCode.Unauthorized)
        //     return response;
        //
        // try
        // {
        //     await _authenticationService.RefreshAsync(cancellationToken);
        // }
        // catch (Exception)
        // {
        //     await _authenticationService.LogoutAsync(cancellationToken);
        //     _navigationManager.NavigateTo("/", true);
        //     return response;
        // }
        //
        // response = await base.SendAsync(request, cancellationToken);
        //
        // if (response.StatusCode != HttpStatusCode.Unauthorized)
        //     return response;
        //
        // await _authenticationService.LogoutAsync(cancellationToken);
        // _navigationManager.NavigateTo("/", true);

        return response;
    }
}