using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace Qweree.WebApplication.Infrastructure.Browser;

public class BrowserCredentialsHandler : DelegatingHandler
{
    public BrowserCredentialsHandler(HttpMessageHandler innerHandler)
        :base(innerHandler)
    {
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
        var response = await base.SendAsync(request, cancellationToken);
        return response;
    }
}