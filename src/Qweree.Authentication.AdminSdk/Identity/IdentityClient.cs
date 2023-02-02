using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Authentication.AdminSdk.Authorization.Roles;
using Qweree.Authentication.AdminSdk.Identity.Clients;
using Qweree.Authentication.AdminSdk.Identity.Users;
using Qweree.Authentication.AdminSdk.Identity.Users.UserInvitation;
using Qweree.Sdk.Http;
using Qweree.Utils;

namespace Qweree.Authentication.AdminSdk.Identity;

public class IdentityClient
{
    private readonly HttpClient _httpClient;

    public IdentityClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ApiResponse<User>> UserGetAsync(Guid id, CancellationToken cancellationToken = new())
    {
        var response = await _httpClient.GetAsync($"users/{id}", cancellationToken);
        return new ApiResponse<User>(response);
    }

    public async Task<ApiResponse<IEnumerable<Role>>> UserEffectiveRolesGetAsync(Guid id, CancellationToken cancellationToken = new())
    {
        var response = await _httpClient.GetAsync($"users/{id}/effective-roles", cancellationToken);
        return new ApiResponse<IEnumerable<Role>>(response);
    }

    public async Task<PaginationApiResponse<User>> UsersPaginateAsync(int skip, int take, Dictionary<string, int> sort, CancellationToken cancellationToken = new())
    {
        var sortString = "";
        foreach (var (field, direction) in sort)
        {
            sortString += $"sort[{field}]={direction}&";
        }
        var queryString = $"?skip={skip}&take={take}&{sortString}";

        var response = await _httpClient.GetAsync($"users/{queryString}", cancellationToken);
        return new PaginationApiResponse<User>(response);
    }

    public async Task<ApiResponse> UserDeleteAsync(Guid id, CancellationToken cancellationToken = new())
    {
        var response = await _httpClient.DeleteAsync($"users/{id}", cancellationToken);
        return new ApiResponse(response);
    }

    public async Task<ApiResponse<UserInvitation>> UserInvitationCreateAsync(UserInvitationInput input, CancellationToken cancellationToken = new())
    {
        var json = JsonUtils.Serialize(input);
        var response = await _httpClient.PostAsync("user-invitations", new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json), cancellationToken);
        return new ApiResponse<UserInvitation>(response);
    }

    public async Task<ApiResponse<UserInvitation>> UserInvitationGetAsync(Guid id, CancellationToken cancellationToken = new())
    {
        var response = await _httpClient.GetAsync($"user-invitations/{id}", cancellationToken);
        return new ApiResponse<UserInvitation>(response);
    }

    public async Task<PaginationApiResponse<UserInvitation>> UserInvitationsPaginateAsync(int skip, int take, Dictionary<string, int> sort, CancellationToken cancellationToken = new())
    {
        var sortString = "";
        foreach (var (field, direction) in sort)
        {
            sortString += $"sort[{field}]={direction}&";
        }
        var queryString = $"?skip={skip}&take={take}&{sortString}";

        var response = await _httpClient.GetAsync($"user-invitations/{queryString}", cancellationToken);
        return new PaginationApiResponse<UserInvitation>(response);
    }

    public async Task<ApiResponse> UserInvitationDeleteAsync(Guid id, CancellationToken cancellationToken = new())
    {
        var response = await _httpClient.DeleteAsync($"user-invitations/{id}", cancellationToken);
        return new ApiResponse(response);
    }

    public async Task<ApiResponse<Client>> ClientGetAsync(Guid id, CancellationToken cancellationToken = new())
    {
        var response = await _httpClient.GetAsync($"clients/{id}", cancellationToken);
        return new ApiResponse<Client>(response);
    }

    public async Task<ApiResponse<RolesCollection>> ClientEffectiveRolesGetAsync(Guid id, CancellationToken cancellationToken = new())
    {
        var response = await _httpClient.GetAsync($"clients/{id}/effective-roles", cancellationToken);
        return new ApiResponse<RolesCollection>(response);
    }

    public async Task<ApiResponse<CreatedClient>> ClientCreateAsync(ClientCreateInput input, CancellationToken cancellationToken = new())
    {
        var json = JsonUtils.Serialize(input);
        var response = await _httpClient.PostAsync("clients", new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json), cancellationToken);
        return new ApiResponse<CreatedClient>(response);
    }

    public async Task<PaginationApiResponse<Client>> ClientsPaginateAsync(int skip, int take, Dictionary<string, int> sort, CancellationToken cancellationToken = new())
    {
        var sortString = "";
        foreach (var (field, direction) in sort)
        {
            sortString += $"sort[{field}]={direction}&";
        }
        var queryString = $"?skip={skip}&take={take}&{sortString}";

        var response = await _httpClient.GetAsync($"clients/{queryString}", cancellationToken);
        return new PaginationApiResponse<Client>(response);
    }

    public async Task<ApiResponse> ClientDeleteAsync(Guid id, CancellationToken cancellationToken = new())
    {
        var response = await _httpClient.DeleteAsync($"clients/{id}", cancellationToken);
        return new ApiResponse(response);
    }
}