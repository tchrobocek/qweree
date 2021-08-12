using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Authentication.Sdk.Tokens;
using Qweree.Sdk.Http.Legacy.Errors;
using Qweree.Utils;

namespace Qweree.Authentication.Sdk.OAuth2
{
    public class OAuth2Client
    {
        private readonly IErrorHandler _errorResponseHandler = new QwereeErrorHandler();
        private readonly HttpClient _httpClient;

        public OAuth2Client(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<TokenInfo> SignInAsync(PasswordGrantInput grantInput, ClientCredentials clientCredentials,
            CancellationToken cancellationToken = new())
        {
            var form = new[]
            {
                new KeyValuePair<string?, string?>("grant_type", "password"),
                new KeyValuePair<string?, string?>("username", grantInput.Username),
                new KeyValuePair<string?, string?>("password", grantInput.Password),
                new KeyValuePair<string?, string?>("client_id", clientCredentials.ClientId),
                new KeyValuePair<string?, string?>("client_secret", clientCredentials.ClientSecret)
            };

            var content = new FormUrlEncodedContent(form);
            var request = new HttpRequestMessage(HttpMethod.Post, string.Empty)
            {
                Content = content
            };

            var response = await _httpClient.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
                await _errorResponseHandler.HandleErrorResponseAsync(response, cancellationToken);

            var tokenInfoDto =
                await response.Content.ReadAsObjectAsync<TokenInfoDto>(JsonUtils.SnakeCaseNamingPolicy,
                    cancellationToken);
            return TokenInfoMapper.FromDto(tokenInfoDto!);
        }
    }
}