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

    public async Task<ApiResponse<IEnumerable<Role>>> RolesFindAsync(CancellationToken cancellationToken = new())
    {
        var response = await _httpClient.GetAsync("roles", cancellationToken);

        return ApiResponse.CreateApiResponse<IEnumerable<Role>>(response);
    }

    public async Task<ApiResponse<Role>> RoleCreateAsync(RoleCreateInput input,
        CancellationToken cancellationToken = new())
    {
        var content = new StringContent(JsonUtils.Serialize(input), Encoding.UTF8, MediaTypeNames.Application.Json);
        var response = await _httpClient.PostAsync("roles", content, cancellationToken);

        return ApiResponse.CreateApiResponse<Role>(response);
    }

    public async Task<ApiResponse<Role>> RoleModifyAsync(Guid id, RoleModifyInput input,
        CancellationToken cancellationToken = new())
    {
        var content = new StringContent(JsonUtils.Serialize(input), Encoding.UTF8, MediaTypeNames.Application.Json);
        var response = await _httpClient.PatchAsync($"roles/{id}", content, cancellationToken);

        return ApiResponse.CreateApiResponse<Role>(response);
    }

    public async Task<ApiResponse<Role>> RoleDeleteAsync(Guid id, CancellationToken cancellationToken = new())
    {
        var response = await _httpClient.DeleteAsync($"roles/{id}", cancellationToken);

        return ApiResponse.CreateApiResponse<Role>(response);
    }
}