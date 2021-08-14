using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Sdk.Http;

namespace Qweree.PiccStash.Sdk
{
    public class PiccClient
    {
        private readonly HttpClient _httpClient;

        public PiccClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ApiResponse<IEnumerable<PiccDto>>> PiccsPaginateAsync(int skip, int take, CancellationToken cancellationToken = new())
        {
            var uri = CreatePaginateUri(skip, take);
            var response = await _httpClient.GetAsync(uri, cancellationToken);

            return ApiResponse.CreateApiResponse<IEnumerable<PiccDto>>(response);
        }

        public async Task<ApiResponse<PiccDto>> PiccUploadAsync(Stream stream, string mediaType, CancellationToken cancellationToken = new())
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
            return ApiResponse.CreateApiResponse<PiccDto>(response);
        }

        public async Task<ApiResponse> DeleteAsync(Guid id, CancellationToken cancellationToken = new())
        {
            var response = await _httpClient.DeleteAsync(id.ToString(), cancellationToken);
            return ApiResponse.CreateApiResponse<PiccDto>(response);
        }

        private string CreatePaginateUri(int skip, int take)
        {
            return $"?skip={skip}&take={take}&sort[CreatedAt]=-1";
        }
    }
}