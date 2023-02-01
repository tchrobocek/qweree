using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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

    public async Task<ApiResponse<IEnumerable<DeviceInfo>>> MyDevicesGetAsync(CancellationToken cancellationToken = new())
    {
        var response = await _httpClient.GetAsync("my-devices", cancellationToken);
        return ApiResponse.CreateApiResponse<IEnumerable<DeviceInfo>>(response);
    }

    public async Task<ApiResponse<MyProfile>> MyProfileGetAsync(CancellationToken cancellationToken = new())
    {
        var response = await _httpClient.GetAsync("", cancellationToken);
        return ApiResponse.CreateApiResponse<MyProfile>(response);
    }

    public async Task<ApiResponse<UserInvitation>> UserInvitationGetAsync(Guid invitation, CancellationToken cancellationToken = new())
    {
        var response = await _httpClient.GetAsync($"register/invitation/{invitation}", cancellationToken);
        return ApiResponse.CreateApiResponse<UserInvitation>(response);
    }

    public async Task<ApiResponse> UserRegisterAsync(UserRegisterInput input, CancellationToken cancellationToken = new())
    {
        var response = await _httpClient.PostAsync("register", new StringContent(JsonUtils.Serialize(input), Encoding.UTF8, MediaTypeNames.Application.Json), cancellationToken);
        return ApiResponse.CreateApiResponse(response);
    }
}