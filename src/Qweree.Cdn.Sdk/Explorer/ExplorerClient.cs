using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Sdk.Http;

namespace Qweree.Cdn.Sdk.Explorer;

public class ExplorerClient
{
    private readonly HttpClient _httpClient;

    public ExplorerClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<JsonApiResponse<IExplorerObjectDto[]>> ExploreAsync(string path, CancellationToken cancellationToken = new())
    {
        var response = await _httpClient.GetAsync(path.Trim('/'), cancellationToken);
        return new JsonApiResponse<IExplorerObjectDto[]>(response);
    }
}