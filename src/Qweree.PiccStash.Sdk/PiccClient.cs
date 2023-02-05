using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Sdk.Http;

namespace Qweree.PiccStash.Sdk;

public class PiccClient
{
    private readonly HttpClient _httpClient;

    public PiccClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<PaginationJsonApiResponse<PiccDto>> PiccsPaginateAsync(int skip, int take, CancellationToken cancellationToken = new())
    {
        var uri = CreatePaginateUri(skip, take);
        var response = await _httpClient.GetAsync(uri, cancellationToken);

        return new PaginationJsonApiResponse<PiccDto>(response);
    }

    public async Task<JsonApiResponse<PiccDto>> PiccUploadAsync(Stream stream, string mediaType, CancellationToken cancellationToken = new())
    {
        var request = new HttpRequestMessage(HttpMethod.Post, string.Empty)
        {
            Content = new StreamContent(stream)
            {
                Headers =
                {
                    {"Content-Type", mediaType}
                }
            }
        };

        var response = await _httpClient.SendAsync(request, cancellationToken);
        return new JsonApiResponse<PiccDto>(response);
    }

    public async Task<ApiResponse> PiccDeleteAsync(Guid id, CancellationToken cancellationToken = new())
    {
        var response = await _httpClient.DeleteAsync(id.ToString(), cancellationToken);
        return new JsonApiResponse<PiccDto>(response);
    }

    public async Task<ApiResponse> PiccReadAsync(Guid id, CancellationToken cancellationToken = new())
    {
        var response = await _httpClient.GetAsync(id.ToString(), HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        return new JsonApiResponse<PiccDto>(response);
    }

    private string CreatePaginateUri(int skip, int take)
    {
        return $"?skip={skip}&take={take}&sort[CreatedAt]=-1";
    }
}