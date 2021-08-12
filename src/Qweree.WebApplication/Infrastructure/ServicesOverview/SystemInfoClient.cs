using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Sdk.Http;

namespace Qweree.WebApplication.Infrastructure.ServicesOverview
{
    public class SystemInfoClient
    {
        private readonly HttpClient _client;

        public SystemInfoClient(HttpClient client)
        {
            _client = client;
        }

        public async Task<ApiResponse<VersionDto>> GetVersionAsync(CancellationToken cancellationToken = new())
        {
            const string uri = "version";
            var response = await _client.GetAsync(uri, cancellationToken);
            return ApiResponse.CreateApiResponse<VersionDto>(response);
        }

        public async Task<ApiResponse<HealthReportDto>> GetHealthAsync(CancellationToken cancellationToken = new())
        {
            const string uri = "health";
            var response = await _client.GetAsync(uri, cancellationToken);
            return ApiResponse.CreateApiResponse<HealthReportDto>(response);
        }
    }
}