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

    public async Task<ApiResponse<UserDto>> UserGetAsync(Guid id, CancellationToken cancellationToken = new())
    {
        var response = await _httpClient.GetAsync($"users/{id}", cancellationToken);
        return new ApiResponse<UserDto>(response);
    }

    public async Task<ApiResponse<IEnumerable<RoleDto>>> UserEffectiveRolesGetAsync(Guid id, CancellationToken cancellationToken = new())
    {
        var response = await _httpClient.GetAsync($"users/{id}/effective-roles", cancellationToken);
        return new ApiResponse<IEnumerable<RoleDto>>(response);
    }

    public async Task<PaginationApiResponse<UserDto>> UsersPaginateAsync(int skip, int take, Dictionary<string, int> sort, CancellationToken cancellationToken = new())
    {
        var sortString = "";
        foreach (var (field, direction) in sort)
        {
            sortString += $"sort[{field}]={direction}&";
        }
        var queryString = $"?skip={skip}&take={take}&{sortString}";

        var response = await _httpClient.GetAsync($"users/{queryString}", cancellationToken);
        return new PaginationApiResponse<UserDto>(response);
    }

    public async Task<ApiResponse> UserDeleteAsync(Guid id, CancellationToken cancellationToken = new())
    {
        var response = await _httpClient.DeleteAsync($"users/{id}", cancellationToken);
        return new ApiResponse(response);
    }

    public async Task<ApiResponse<UserInvitationDescriptorDto>> UserInvitationCreateAsync(UserInvitationInputDto input, CancellationToken cancellationToken = new())
    {
        var json = JsonUtils.Serialize(input);
        var response = await _httpClient.PostAsync("user-invitations", new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json), cancellationToken);
        return new ApiResponse<UserInvitationDescriptorDto>(response);
    }

    public async Task<ApiResponse<UserInvitationDescriptorDto>> UserInvitationGetAsync(Guid id, CancellationToken cancellationToken = new())
    {
        var response = await _httpClient.GetAsync($"user-invitations/{id}", cancellationToken);
        return new ApiResponse<UserInvitationDescriptorDto>(response);
    }

    public async Task<PaginationApiResponse<UserInvitationDescriptorDto>> UserInvitationsPaginateAsync(int skip, int take, Dictionary<string, int> sort, CancellationToken cancellationToken = new())
    {
        var sortString = "";
        foreach (var (field, direction) in sort)
        {
            sortString += $"sort[{field}]={direction}&";
        }
        var queryString = $"?skip={skip}&take={take}&{sortString}";

        var response = await _httpClient.GetAsync($"user-invitations/{queryString}", cancellationToken);
        return new PaginationApiResponse<UserInvitationDescriptorDto>(response);
    }

    public async Task<ApiResponse> UserInvitationDeleteAsync(Guid id, CancellationToken cancellationToken = new())
    {
        var response = await _httpClient.DeleteAsync($"user-invitations/{id}", cancellationToken);
        return new ApiResponse(response);
    }

    public async Task<ApiResponse<ClientDto>> ClientGetAsync(Guid id, CancellationToken cancellationToken = new())
    {
        var response = await _httpClient.GetAsync($"clients/{id}", cancellationToken);
        return new ApiResponse<ClientDto>(response);
    }

    public async Task<ApiResponse<ClientEffectiveRolesCollectionDto>> ClientEffectiveRolesGetAsync(Guid id, CancellationToken cancellationToken = new())
    {
        var response = await _httpClient.GetAsync($"clients/{id}/effective-roles", cancellationToken);
        return new ApiResponse<ClientEffectiveRolesCollectionDto>(response);
    }

    public async Task<ApiResponse<CreatedClientDto>> ClientCreateAsync(ClientCreateInputDto input, CancellationToken cancellationToken = new())
    {
        var json = JsonUtils.Serialize(input);
        var response = await _httpClient.PostAsync("clients", new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json), cancellationToken);
        return new ApiResponse<CreatedClientDto>(response);
    }

    public async Task<PaginationApiResponse<ClientDto>> ClientsPaginateAsync(int skip, int take, Dictionary<string, int> sort, CancellationToken cancellationToken = new())
    {
        var sortString = "";
        foreach (var (field, direction) in sort)
        {
            sortString += $"sort[{field}]={direction}&";
        }
        var queryString = $"?skip={skip}&take={take}&{sortString}";

        var response = await _httpClient.GetAsync($"clients/{queryString}", cancellationToken);
        return new PaginationApiResponse<ClientDto>(response);
    }

    public async Task<ApiResponse> ClientDeleteAsync(Guid id, CancellationToken cancellationToken = new())
    {
        var response = await _httpClient.DeleteAsync($"clients/{id}", cancellationToken);
        return new ApiResponse(response);
    }
}