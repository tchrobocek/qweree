using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Authentication.Sdk.Exceptions;
using Qweree.Sdk;
using Qweree.Utils;

namespace Qweree.Authentication.Sdk.Authentication
{
    public class OAuth2Adapter
    {
        private readonly Uri _authUri;
        private readonly HttpClient _httpClient;

        public OAuth2Adapter(Uri authUri, HttpClient httpClient)
        {
            _authUri = authUri;
            _httpClient = httpClient;
        }

        public async Task<TokenInfo> SignInAsync(PasswordGrantInput grantInput, CancellationToken cancellationToken = new CancellationToken())
        {
            var form = new[]
            {
                new KeyValuePair<string?, string?>("grant_type", "password"),
                new KeyValuePair<string?, string?>("username", grantInput.Username),
                new KeyValuePair<string?, string?>("password", grantInput.Password)
            };

            var content = new FormUrlEncodedContent(form);
            var request = new HttpRequestMessage(HttpMethod.Post, _authUri)
            {
                Content = content
            };

            var response = await _httpClient.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
                await HandleResponseAsync(response, cancellationToken);

            var tokenInfoDto = await response.Content.ReadAsObjectAsync<TokenInfoDto>(JsonUtils.SnakeCaseNamingPolicy, cancellationToken);
            return TokenInfoMapper.FromDto(tokenInfoDto!);
        }

        private async Task HandleResponseAsync(HttpResponseMessage response, CancellationToken cancellationToken)
        {
            var errorResponse = await response.Content.ReadAsObjectAsync<ErrorResponseDto>(cancellationToken);

            if ((int)response.StatusCode >= 400 && (int)response.StatusCode < 500)
                throw new ClientErrorException((int)response.StatusCode, errorResponse!);

            throw new ServerErrorException((int)response.StatusCode, errorResponse!);
        }
    }
}