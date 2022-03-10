using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Qweree.ConsoleApplication.Infrastructure.RunContext;
using Qweree.PiccStash.Sdk;

namespace Qweree.ConsoleApplication.Infrastructure.Commands;

public class PiccClientFactory
{
    private readonly HttpMessageHandlerFactory _handlerFactory;
    private readonly Context _context;

    public PiccClientFactory(HttpMessageHandlerFactory handlerFactory, Context context)
    {
        _handlerFactory = handlerFactory;
        _context = context;
    }

    public async Task<PiccClient> CreateClientAsync(CancellationToken cancellationToken = new())
    {
        var config = await _context.GetConfigurationAsync(cancellationToken);
        var httpClient = new HttpClient(await _handlerFactory.CreateHandlerAsync(cancellationToken))
        {
            BaseAddress = new Uri(new Uri(config.PiccUri!), "api/v1/picc/")
        };

        return new PiccClient(httpClient);
    }
}