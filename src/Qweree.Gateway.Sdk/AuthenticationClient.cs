using System;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Authentication.Sdk.Session;
using Qweree.Sdk.Http;
using Qweree.Utils;

namespace Qweree.Gateway.Sdk;

public class AuthenticationClient
{
    private readonly HttpClient _httpClient;

    public AuthenticationClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<JsonApiResponse<Identity>> LoginAsync(LoginInputDto input,
        CancellationToken cancellationToken = new())
    {
        var content = new StringContent(JsonUtils.Serialize(input), Encoding.UTF8, MediaTypeNames.Application.Json);
        var response = await _httpClient.PostAsync("login", content, cancellationToken);

        return new JsonApiResponse<Identity>(response);
    }

    public async Task<JsonApiResponse<Identity>> RefreshAsync(CancellationToken cancellationToken = new())
    {
        var response = await _httpClient.PostAsync("refresh", new ByteArrayContent(Array.Empty<byte>()), cancellationToken);
        return new JsonApiResponse<Identity>(response);
    }

    public async Task<ApiResponse> LogoutAsync(CancellationToken cancellationToken = new())
    {
        var response = await _httpClient.PostAsync("logout", new ByteArrayContent(Array.Empty<byte>()), cancellationToken);
        return new ApiResponse(response);
    }

    public async Task<ApiResponse> RevokeSessionAsync(Guid sessionId, CancellationToken cancellationToken = new())
    {
        var response = await _httpClient.DeleteAsync($"session/{sessionId}", cancellationToken);
        return new ApiResponse(response);
    }
}