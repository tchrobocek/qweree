using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Sdk.Http;

namespace Qweree.Cdn.Sdk.System;

public class StatsClient
{
    private readonly HttpClient _httpClient;

    public StatsClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<JsonApiResponse<CdnStatsDto>> GetStatsAsync(CancellationToken cancellationToken = new())
    {
        var response = await _httpClient.GetAsync(string.Empty, cancellationToken);
        return new JsonApiResponse<CdnStatsDto>(response);
    }
}