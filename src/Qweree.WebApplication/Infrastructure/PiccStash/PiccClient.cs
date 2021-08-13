using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Qweree.Sdk.Http;

namespace Qweree.WebApplication.Infrastructure.PiccStash
{
    public class PiccClient
    {
        private readonly HttpClient _httpClient;

        public PiccClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ApiResponse<IEnumerable<PiccDto>>> PaginatePics(int skip, int take)
        {
            var uri = CreatePaginateUri(skip, take);
            var response = await _httpClient.GetAsync(uri);

            return ApiResponse.CreateApiResponse<IEnumerable<PiccDto>>(response);
        }

        private string CreatePaginateUri(int skip, int take)
        {
            return $"?skip={skip}&take={take}&sort[CreatedAt]=1";
        }
    }
}