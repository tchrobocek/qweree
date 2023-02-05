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

namespace Qweree.Authentication.AdminSdk;

public class AdminSdkClient
{
    private readonly HttpClient _httpClient;

    public AdminSdkClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    #region User

    public async Task<JsonApiResponse<User>> UserGetAsync(Guid id, CancellationToken cancellationToken = new())
    {
        var response = await _httpClient.GetAsync($"identity/users/{id}", cancellationToken);
        return new JsonApiResponse<User>(response, JsonHelper.CamelCaseOptions);
    }

    public async Task<JsonApiResponse<IEnumerable<Role>>> UserEffectiveRolesGetAsync(Guid id,
        CancellationToken cancellationToken = new())
    {
        var response = await _httpClient.GetAsync($"identity/users/{id}/effective-roles", cancellationToken);
        return new JsonApiResponse<IEnumerable<Role>>(response, JsonHelper.CamelCaseOptions);
    }

    public async Task<PaginationJsonApiResponse<User>> UsersPaginateAsync(int skip, int take, Dictionary<string, int> sort,
        CancellationToken cancellationToken = new())
    {
        var sortString = "";
        foreach (var (field, direction) in sort)
        {
            sortString += $"sort[{field}]={direction}&";
        }

        var queryString = $"?skip={skip}&take={take}&{sortString}";

        var response = await _httpClient.GetAsync($"identity/users/{queryString}", cancellationToken);
        return new PaginationJsonApiResponse<User>(response, JsonHelper.CamelCaseOptions);
    }

    public async Task<ApiResponse> UserDeleteAsync(Guid id, CancellationToken cancellationToken = new())
    {
        var response = await _httpClient.DeleteAsync($"identity/users/{id}", cancellationToken);
        return new ApiResponse(response);
    }

    public async Task<JsonApiResponse<UserInvitation>> UserInvitationCreateAsync(UserInvitationInput input,
        CancellationToken cancellationToken = new())
    {
        var json = JsonHelper.Serialize(input);
        var response = await _httpClient.PostAsync("identity/user-invitations",
            new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json), cancellationToken);
        return new JsonApiResponse<UserInvitation>(response, JsonHelper.CamelCaseOptions);
    }

    public async Task<JsonApiResponse<UserInvitation>> UserInvitationGetAsync(Guid id,
        CancellationToken cancellationToken = new())
    {
        var response = await _httpClient.GetAsync($"identity/user-invitations/{id}", cancellationToken);
        return new JsonApiResponse<UserInvitation>(response, JsonHelper.CamelCaseOptions);
    }

    public async Task<PaginationJsonApiResponse<UserInvitation>> UserInvitationsPaginateAsync(int skip, int take,
        Dictionary<string, int> sort, CancellationToken cancellationToken = new())
    {
        var sortString = "";
        foreach (var (field, direction) in sort)
        {
            sortString += $"sort[{field}]={direction}&";
        }

        var queryString = $"?skip={skip}&take={take}&{sortString}";

        var response = await _httpClient.GetAsync($"identity/user-invitations/{queryString}", cancellationToken);
        return new PaginationJsonApiResponse<UserInvitation>(response, JsonHelper.CamelCaseOptions);
    }

    public async Task<ApiResponse> UserInvitationDeleteAsync(Guid id, CancellationToken cancellationToken = new())
    {
        var response = await _httpClient.DeleteAsync($"identity/user-invitations/{id}", cancellationToken);
        return new ApiResponse(response);
    }

    #endregion

    #region Client

    public async Task<JsonApiResponse<Client>> ClientGetAsync(Guid id, CancellationToken cancellationToken = new())
    {
        var response = await _httpClient.GetAsync($"identity/clients/{id}", cancellationToken);
        return new JsonApiResponse<Client>(response, JsonHelper.CamelCaseOptions);
    }

    public async Task<JsonApiResponse<ClientWithSecret>> ClientCreateAsync(ClientCreateInput input,
        CancellationToken cancellationToken = new())
    {
        var json = JsonHelper.Serialize(input);
        var response = await _httpClient.PostAsync("identity/clients",
            new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json), cancellationToken);
        return new JsonApiResponse<ClientWithSecret>(response, JsonHelper.CamelCaseOptions);
    }

    public async Task<PaginationJsonApiResponse<Client>> ClientsPaginateAsync(int skip, int take,
        Dictionary<string, int> sort, CancellationToken cancellationToken = new())
    {
        var sortString = "";
        foreach (var (field, direction) in sort)
        {
            sortString += $"sort[{field}]={direction}&";
        }

        var queryString = $"?skip={skip}&take={take}&{sortString}";

        var response = await _httpClient.GetAsync($"identity/clients/{queryString}", cancellationToken);
        return new PaginationJsonApiResponse<Client>(response, JsonHelper.CamelCaseOptions);
    }

    public async Task<ApiResponse> ClientDeleteAsync(Guid id, CancellationToken cancellationToken = new())
    {
        var response = await _httpClient.DeleteAsync($"identity/clients/{id}", cancellationToken);
        return new ApiResponse(response);
    }

    public async Task<JsonApiResponse<Client>> ClientModifyAsync(Guid id, ClientModifyInput input,
        CancellationToken cancellationToken = new())
    {
        var json = JsonHelper.Serialize(input);
        var response = await _httpClient.PatchAsync($"identity/clients/{id}",
            new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json), cancellationToken);
        return new JsonApiResponse<Client>(response, JsonHelper.CamelCaseOptions);
    }

    public async Task<JsonApiResponse<ClientWithSecret>> ClientSecretRegenerateAsync(Guid id,
        CancellationToken cancellationToken = new())
    {
        var response = await _httpClient.PostAsync($"identity/clients/{id}/regenerate-secret",
            new ByteArrayContent(Array.Empty<byte>()), cancellationToken);
        return new JsonApiResponse<ClientWithSecret>(response, JsonHelper.CamelCaseOptions);
    }

    public async Task<JsonApiResponse<Client>> ClientAccessDefinitionsReplaceAsync(Guid id, IEnumerable<IAccessDefinitionInput> input,
        CancellationToken cancellationToken = new())
    {
        var json = JsonHelper.Serialize(input);
        var response = await _httpClient.PutAsync($"identity/clients/{id}/access-definitions",
            new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json), cancellationToken);
        return new JsonApiResponse<Client>(response, JsonHelper.CamelCaseOptions);
    }

    #endregion

    #region Role

    public async Task<JsonApiResponse<IEnumerable<Role>>> RolesFindAsync(CancellationToken cancellationToken = new())
    {
        var response = await _httpClient.GetAsync("authorization/roles", cancellationToken);

        return new JsonApiResponse<IEnumerable<Role>>(response, JsonHelper.CamelCaseOptions);
    }

    public async Task<JsonApiResponse<Role>> RoleCreateAsync(RoleCreateInput input,
        CancellationToken cancellationToken = new())
    {
        var content = new StringContent(JsonHelper.Serialize(input), Encoding.UTF8, MediaTypeNames.Application.Json);
        var response = await _httpClient.PostAsync("authorization/roles", content, cancellationToken);

        return new JsonApiResponse<Role>(response, JsonHelper.CamelCaseOptions);
    }

    public async Task<JsonApiResponse<Role>> RoleModifyAsync(Guid id, RoleModifyInput input,
        CancellationToken cancellationToken = new())
    {
        var content = new StringContent(JsonHelper.Serialize(input), Encoding.UTF8, MediaTypeNames.Application.Json);
        var response = await _httpClient.PatchAsync($"authorization/roles/{id}", content, cancellationToken);

        return new JsonApiResponse<Role>(response, JsonHelper.CamelCaseOptions);
    }

    public async Task<ApiResponse> RoleDeleteAsync(Guid id, CancellationToken cancellationToken = new())
    {
        var response = await _httpClient.DeleteAsync($"authorization/roles/{id}", cancellationToken);

        return new ApiResponse(response);
    }

    #endregion
}