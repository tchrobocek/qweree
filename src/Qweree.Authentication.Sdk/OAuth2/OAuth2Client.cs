using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Sdk.Http;

namespace Qweree.Authentication.Sdk.OAuth2;

public class OAuth2Client
{
    private readonly HttpClient _httpClient;

    public OAuth2Client(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ApiResponse<TokenInfo>> SignInAsync(PasswordGrantInput grantInput, ClientCredentials clientCredentials,
        OAuth2RequestOptions options = default, CancellationToken cancellationToken = new())
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
        var request = new HttpRequestMessage(HttpMethod.Post, "auth")
        {
            Content = content
        };

        if (options.UserAgentHeader is not null)
            request.Headers.Add("User-Agent", options.UserAgentHeader);

        var response = await _httpClient.SendAsync(request, cancellationToken);
        return ApiResponse.CreateApiResponse<TokenInfo>(response);
    }

    public async Task<ApiResponse<TokenInfo>> SignInAsync(ClientCredentials clientCredentials,
        OAuth2RequestOptions options = default, CancellationToken cancellationToken = new())
    {
        var form = new[]
        {
            new KeyValuePair<string?, string?>("grant_type", "client_credentials"),
            new KeyValuePair<string?, string?>("client_id", clientCredentials.ClientId),
            new KeyValuePair<string?, string?>("client_secret", clientCredentials.ClientSecret)
        };

        var content = new FormUrlEncodedContent(form);
        var request = new HttpRequestMessage(HttpMethod.Post, "auth")
        {
            Content = content
        };

        if (options.UserAgentHeader is not null)
            request.Headers.Add("User-Agent", options.UserAgentHeader);

        var response = await _httpClient.SendAsync(request, cancellationToken);
        return ApiResponse.CreateApiResponse<TokenInfo>(response);
    }

    public async Task<ApiResponse<TokenInfo>> RefreshAsync(RefreshTokenGrantInput refreshTokenGrantInput, ClientCredentials clientCredentials,
        OAuth2RequestOptions options = default, CancellationToken cancellationToken = new())
    {
        var form = new[]
        {
            new KeyValuePair<string?, string?>("grant_type", "refresh_token"),
            new KeyValuePair<string?, string?>("refresh_token", refreshTokenGrantInput.RefreshToken),
            new KeyValuePair<string?, string?>("client_id", clientCredentials.ClientId),
            new KeyValuePair<string?, string?>("client_secret", clientCredentials.ClientSecret)
        };

        var content = new FormUrlEncodedContent(form);
        var request = new HttpRequestMessage(HttpMethod.Post, "auth")
        {
            Content = content
        };

        if (options.UserAgentHeader is not null)
            request.Headers.Add("User-Agent", options.UserAgentHeader);

        var response = await _httpClient.SendAsync(request, cancellationToken);
        return ApiResponse.CreateApiResponse<TokenInfo>(response);
    }

    public async Task<ApiResponse> RevokeAsync(CancellationToken cancellationToken = new())
    {
        var response = await _httpClient.PostAsync("revoke", new ByteArrayContent(Array.Empty<byte>()), cancellationToken);
        return ApiResponse.CreateApiResponse(response);
    }
}

public struct OAuth2RequestOptions
{
    public OAuth2RequestOptions(string? userAgentHeader)
    {
        UserAgentHeader = userAgentHeader;
    }

    public string? UserAgentHeader { get; }
}