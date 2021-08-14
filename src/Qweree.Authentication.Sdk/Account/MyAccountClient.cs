using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Sdk.Http;
using Qweree.Utils;

namespace Qweree.Authentication.Sdk.Account
{
    public class PasswordClient
    {
        private readonly HttpClient _httpClient;

        public PasswordClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ApiResponse> ChangeMyPasswordAsync(ChangeMyPasswordInput input, CancellationToken cancellationToken = new())
        {
            var response = await _httpClient.PostAsync("api/account/change-password",
                new StringContent(JsonUtils.Serialize(input), Encoding.UTF8, MediaTypeNames.Application.Json),
                cancellationToken);

            return ApiResponse.CreateApiResponse(response);
        }
    }
}