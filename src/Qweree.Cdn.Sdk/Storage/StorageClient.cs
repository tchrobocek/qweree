using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Sdk.Http;

namespace Qweree.Cdn.Sdk.Storage;

public class StorageClient
{
    private readonly HttpClient _httpClient;

    public StorageClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ApiResponse<StoredObjectDescriptorDto>> StoreAsync(string path, string mediaType, Stream stream,
        bool force = false, bool isPrivateResource = true, CancellationToken cancellationToken = new())
    {
        var method = HttpMethod.Post;
        if (force)
            method = HttpMethod.Put;

        var isPrivate = "true";
        if (!isPrivateResource)
            isPrivate = "false";

        var request = new HttpRequestMessage(method, path.Trim('/'))
        {
            Content = new StreamContent(stream)
            {
                Headers =
                {
                    {"Content-Type", mediaType},
                    {"X-Private", isPrivate}
                }
            }
        };

        var response = await _httpClient.SendAsync(request, cancellationToken);
        return ApiResponse.CreateApiResponse<StoredObjectDescriptorDto>(response);
    }

    public async Task<ApiResponse> RetrieveAsync(string path, string? ifNoneMatch = null, CancellationToken cancellationToken = new())
    {
        var request = new HttpRequestMessage(HttpMethod.Get, path.Trim('/'));

        if (ifNoneMatch != null)
            request.Headers.IfNoneMatch.Add(new EntityTagHeaderValue($@"""{ifNoneMatch.Trim('"')}"""));

        var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        return ApiResponse.CreateApiResponse(response);
    }

    public async Task<ApiResponse> DeleteAsync(string path, CancellationToken cancellationToken = new())
    {
        var response = await _httpClient.DeleteAsync(path.Trim('/'), cancellationToken);
        return ApiResponse.CreateApiResponse(response);
    }
}