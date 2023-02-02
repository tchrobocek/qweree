using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Authentication.AdminSdk.Authorization.Roles;
using Qweree.Sdk.Http;
using Qweree.Utils;

namespace Qweree.Authentication.AdminSdk.Authorization;

public class AuthorizationClient
{
    private readonly HttpClient _httpClient;

    public AuthorizationClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ApiResponse<IEnumerable<ClientRole>>> ClientRolesFindAsync(CancellationToken cancellationToken = new())
    {
        var response = await _httpClient.GetAsync("client-roles", cancellationToken);

        return ApiResponse.CreateApiResponse<IEnumerable<ClientRole>>(response);
    }

    public async Task<ApiResponse<ClientRole>> ClientRoleCreateAsync(ClientRoleCreateInput input,
        CancellationToken cancellationToken = new())
    {
        var content = new StringContent(JsonUtils.Serialize(input), Encoding.UTF8, MediaTypeNames.Application.Json);
        var response = await _httpClient.PostAsync("client-roles", content, cancellationToken);

        return ApiResponse.CreateApiResponse<ClientRole>(response);
    }

    public async Task<ApiResponse<ClientRole>> ClientRoleModifyAsync(Guid id, ClientRoleModifyInput input,
        CancellationToken cancellationToken = new())
    {
        var content = new StringContent(JsonUtils.Serialize(input), Encoding.UTF8, MediaTypeNames.Application.Json);
        var response = await _httpClient.PatchAsync($"client-roles/{id}", content, cancellationToken);

        return ApiResponse.CreateApiResponse<ClientRole>(response);
    }

    public async Task<ApiResponse<ClientRole>> ClientRoleDeleteAsync(Guid id, CancellationToken cancellationToken = new())
    {
        var response = await _httpClient.DeleteAsync($"client-roles/{id}", cancellationToken);

        return ApiResponse.CreateApiResponse<ClientRole>(response);
    }

    public async Task<ApiResponse<IEnumerable<UserRole>>> UserRolesFindAsync(CancellationToken cancellationToken = new())
    {
        var response = await _httpClient.GetAsync("user-roles", cancellationToken);

        return ApiResponse.CreateApiResponse<IEnumerable<UserRole>>(response);
    }

    public async Task<ApiResponse<UserRole>> UserRoleCreateAsync(UserRoleCreateInput input,
        CancellationToken cancellationToken = new())
    {
        var content = new StringContent(JsonUtils.Serialize(input), Encoding.UTF8, MediaTypeNames.Application.Json);
        var response = await _httpClient.PostAsync("user-roles", content, cancellationToken);

        return ApiResponse.CreateApiResponse<UserRole>(response);
    }

    public async Task<ApiResponse<UserRole>> UserRoleModifyAsync(Guid id, UserRoleModifyInput input,
        CancellationToken cancellationToken = new())
    {
        var content = new StringContent(JsonUtils.Serialize(input), Encoding.UTF8, MediaTypeNames.Application.Json);
        var response = await _httpClient.PatchAsync($"user-roles/{id}", content, cancellationToken);

        return ApiResponse.CreateApiResponse<UserRole>(response);
    }

    public async Task<ApiResponse<UserRole>> UserRoleDeleteAsync(Guid id, CancellationToken cancellationToken = new())
    {
        var response = await _httpClient.DeleteAsync($"user-roles/{id}", cancellationToken);

        return ApiResponse.CreateApiResponse<UserRole>(response);
    }
}