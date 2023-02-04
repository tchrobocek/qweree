using System;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Authentication.Sdk.Account.MyAccount;
using Qweree.Authentication.Sdk.Account.UserRegister;
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

        return ApiResponse.CreateApiResponse(response);
    }

    public async Task<ApiResponse<MyProfile>> MyProfileGetAsync(CancellationToken cancellationToken = new())
    {
        var response = await _httpClient.GetAsync("", cancellationToken);
        return ApiResponse.CreateApiResponse<MyProfile>(response);
    }

    public async Task<ApiResponse<MyAccountSessionInfo[]>> MySessionsGetAsync(CancellationToken cancellationToken = new())
    {
        var response = await _httpClient.GetAsync("sessions", cancellationToken);
        return ApiResponse.CreateApiResponse<MyAccountSessionInfo[]>(response);
    }

    public async Task<ApiResponse<AuthUserInvitation>> UserInvitationGetAsync(Guid invitation, CancellationToken cancellationToken = new())
    {
        var response = await _httpClient.GetAsync($"register/invitation/{invitation}", cancellationToken);
        return ApiResponse.CreateApiResponse<AuthUserInvitation>(response);
    }

    public async Task<ApiResponse> UserRegisterAsync(UserRegisterInput input, CancellationToken cancellationToken = new())
    {
        var response = await _httpClient.PostAsync("register", new StringContent(JsonUtils.Serialize(input), Encoding.UTF8, MediaTypeNames.Application.Json), cancellationToken);
        return ApiResponse.CreateApiResponse(response);
    }

    public async Task<ApiResponse> RevokeSessionAsync(Guid sessionId, CancellationToken cancellationToken = new())
    {
        var response = await _httpClient.DeleteAsync($"sessions/{sessionId}", cancellationToken);
        return ApiResponse.CreateApiResponse(response);
    }
}