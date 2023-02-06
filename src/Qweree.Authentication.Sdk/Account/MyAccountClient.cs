using System;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Qweree.Authentication.Sdk.Account.MyAccount;
using Qweree.Authentication.Sdk.Account.UserRegister;
using Qweree.Authentication.Sdk.Identity;
using Qweree.Authentication.Sdk.OAuth2;
using Qweree.Sdk.Http;
using Qweree.Utils;

namespace Qweree.Authentication.Sdk.Account;

public class MyAccountClient
{
    private readonly HttpClient _httpClient;

    public MyAccountClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ApiResponse> ChangeMyPasswordAsync(ChangeMyPasswordInput input, CancellationToken cancellationToken = new())
    {
        var response = await _httpClient.PostAsync("change-password",
            new StringContent(JsonUtils.Serialize(input), Encoding.UTF8, MediaTypeNames.Application.Json),
            cancellationToken);

        return new ApiResponse(response);
    }

    public async Task<JsonApiResponse<MyProfile>> MyProfileGetAsync(CancellationToken cancellationToken = new())
    {
        var response = await _httpClient.GetAsync("", cancellationToken);
        return new JsonApiResponse<MyProfile>(response);
    }

    public async Task<JsonApiResponse<MyAccountSessionInfo[]>> MySessionsGetAsync(CancellationToken cancellationToken = new())
    {
        var response = await _httpClient.GetAsync("sessions", cancellationToken);
        return new JsonApiResponse<MyAccountSessionInfo[]>(response);
    }

    public async Task<JsonApiResponse<AuthUserInvitation>> UserInvitationGetAsync(Guid invitation, CancellationToken cancellationToken = new())
    {
        var response = await _httpClient.GetAsync($"register/invitation/{invitation}", cancellationToken);
        return new JsonApiResponse<AuthUserInvitation>(response);
    }

    public async Task<ApiResponse> UserRegisterAsync(UserRegisterInput input, CancellationToken cancellationToken = new())
    {
        var response = await _httpClient.PostAsync("register", new StringContent(JsonUtils.Serialize(input), Encoding.UTF8, MediaTypeNames.Application.Json), cancellationToken);
        return new ApiResponse(response);
    }

    public async Task<ApiResponse> RevokeSessionAsync(Guid sessionId, CancellationToken cancellationToken = new())
    {
        var response = await _httpClient.DeleteAsync($"sessions/{sessionId}", cancellationToken);
        return new ApiResponse(response);
    }

    public async Task<JsonApiResponse<AuthClient>> ApplicationConsentInfoGetAsync(string clientId, CancellationToken cancellationToken = new())
    {
        var response = await _httpClient.GetAsync($"application-consent/{clientId}", cancellationToken);
        return new JsonApiResponse<AuthClient>(response);
    }

    public async Task<JsonApiResponse<TokenInfo>> ApplicationConsentAsync(string clientId, string redirectUri, CancellationToken cancellationToken = new())
    {
        var response = await _httpClient.PostAsync($"application-consent/{clientId}?redirect_uri={HttpUtility.UrlEncode(redirectUri)}", new ByteArrayContent(Array.Empty<byte>()), cancellationToken);
        return new JsonApiResponse<TokenInfo>(response);
    }
}