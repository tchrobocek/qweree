using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Authentication.Sdk.Http;
using Qweree.ConsoleApplication.Infrastructure.RunContext;
using Qweree.PiccStash.Sdk;

namespace Qweree.ConsoleApplication.Infrastructure.Commands;

public class PiccClientFactory
{
    private readonly Context _context;
    private readonly AuthorizationHeaderHandler _qwereeHandler;

    public PiccClientFactory(HttpMessageHandler innerHandler, ITokenStorage tokenStorage, Context context)
    {
        _context = context;
        _qwereeHandler = new AuthorizationHeaderHandler(innerHandler, tokenStorage);
    }

    public async Task<PiccClient> CreateClientAsync(CancellationToken cancellationToken = new())
    {
        var config = await _context.GetConfigurationAsync(cancellationToken);
        var httpClient = new HttpClient(_qwereeHandler)
        {
            BaseAddress = new Uri(new Uri(config.PiccUri!), "api/v1/picc/")
        };

        return new PiccClient(httpClient);
    }
}