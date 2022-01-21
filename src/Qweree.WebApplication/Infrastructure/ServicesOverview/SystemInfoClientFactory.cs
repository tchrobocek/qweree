using System;
using System.Net.Http;
using Qweree.WebApplication.Infrastructure.Browser;

namespace Qweree.WebApplication.Infrastructure.ServicesOverview;

public class SystemInfoClientFactory
{
    private readonly UnauthorizedHttpHandler _httpHandler;

    public SystemInfoClientFactory(UnauthorizedHttpHandler httpHandler)
    {
        _httpHandler = httpHandler;
    }

    public SystemInfoClient Create(Uri baseUri)
    {
        var client = new HttpClient(_httpHandler)
        {
            BaseAddress = new Uri(baseUri, "api/system/")
        };

        return new SystemInfoClient(client);
    }
}