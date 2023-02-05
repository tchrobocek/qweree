using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Sdk.Http;

namespace Qweree.WebApplication.Infrastructure.ServicesOverview;

public class SystemInfoClient
{
    private readonly HttpClient _client;

    public SystemInfoClient(HttpClient client)
    {
        _client = client;
    }

    public async Task<JsonApiResponse<VersionDto>> GetVersionAsync(CancellationToken cancellationToken = new())
    {
        const string uri = "version";
        var response = await _client.GetAsync(uri, cancellationToken);
        return new JsonApiResponse<VersionDto>(response);
    }

    public async Task<JsonApiResponse<HealthReportDto>> GetHealthAsync(CancellationToken cancellationToken = new())
    {
        const string uri = "health";
        var response = await _client.GetAsync(uri, cancellationToken);
        return new JsonApiResponse<HealthReportDto>(response);
    }
}