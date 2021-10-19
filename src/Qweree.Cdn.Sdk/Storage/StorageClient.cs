using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Sdk.Http;

namespace Qweree.Cdn.Sdk.Storage
{
    public class StorageClient
    {
        private readonly HttpClient _httpClient;

        public StorageClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ApiResponse<StoredObjectDescriptorDto>> StoreAsync(string path, string mediaType, Stream stream,
            CancellationToken cancellationToken = new())
        {
            var request = new HttpRequestMessage(HttpMethod.Put, path.Trim('/'))
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
            return ApiResponse.CreateApiResponse<StoredObjectDescriptorDto>(response);
        }

        public async Task<ApiResponse> RetrieveAsync(string path, CancellationToken cancellationToken = new())
        {
            var response = await _httpClient.GetAsync(path.Trim('/'), HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            return ApiResponse.CreateApiResponse(response);
        }

        public async Task<ApiResponse> DeleteAsync(string path, CancellationToken cancellationToken = new())
        {
            var response = await _httpClient.DeleteAsync(path.Trim('/'), cancellationToken);
            return ApiResponse.CreateApiResponse(response);
        }
    }
}