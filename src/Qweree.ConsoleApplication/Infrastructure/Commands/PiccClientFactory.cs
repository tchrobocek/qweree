using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Qweree.ConsoleApplication.Infrastructure.RunContext;
using Qweree.PiccStash.Sdk;
using Qweree.Sdk.Http.HttpClient;

namespace Qweree.ConsoleApplication.Infrastructure.Commands;

public class PiccClientFactory
{
    private readonly Context _context;
    private readonly QwereeHttpHandler _qwereeHandler;

    public PiccClientFactory(HttpMessageHandler innerHandler, ITokenStorage tokenStorage, Context context)
    {
        _context = context;
        _qwereeHandler = new QwereeHttpHandler(innerHandler, tokenStorage);
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