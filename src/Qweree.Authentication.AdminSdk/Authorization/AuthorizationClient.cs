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

namespace Qweree.Authentication.AdminSdk.Authorization
{
    public class AuthorizationClient
    {
        private readonly HttpClient _httpClient;

        public AuthorizationClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ApiResponse<IEnumerable<ClientRoleDto>>> ClientRolesFindAsync(CancellationToken cancellationToken = new())
        {
            var response = await _httpClient.GetAsync("clientRoles", cancellationToken);

            return ApiResponse.CreateApiResponse<IEnumerable<ClientRoleDto>>(response);
        }

        public async Task<ApiResponse<ClientRoleDto>> ClientRoleCreateAsync(ClientRoleCreateInputDto input,
            CancellationToken cancellationToken = new())
        {
            var content = new StringContent(JsonUtils.Serialize(input), Encoding.UTF8, MediaTypeNames.Application.Json);
            var response = await _httpClient.PostAsync("clientRoles", content, cancellationToken);

            return ApiResponse.CreateApiResponse<ClientRoleDto>(response);
        }

        public async Task<ApiResponse<ClientRoleDto>> ClientRoleModifyAsync(Guid id, ClientRoleModifyInputDto input,
            CancellationToken cancellationToken = new())
        {
            var content = new StringContent(JsonUtils.Serialize(input), Encoding.UTF8, MediaTypeNames.Application.Json);
            var response = await _httpClient.PatchAsync($"clientRoles/{id}", content, cancellationToken);

            return ApiResponse.CreateApiResponse<ClientRoleDto>(response);
        }

        public async Task<ApiResponse<ClientRoleDto>> ClientRoleDeleteAsync(Guid id, CancellationToken cancellationToken = new())
        {
            var response = await _httpClient.DeleteAsync($"clientRoles{id}", cancellationToken);

            return ApiResponse.CreateApiResponse<ClientRoleDto>(response);
        }

        public async Task<ApiResponse<IEnumerable<UserRoleDto>>> UserRolesFindAsync(CancellationToken cancellationToken = new())
        {
            var response = await _httpClient.GetAsync("userRoles", cancellationToken);

            return ApiResponse.CreateApiResponse<IEnumerable<UserRoleDto>>(response);
        }

        public async Task<ApiResponse<UserRoleDto>> UserRoleCreateAsync(UserRoleCreateInputDto input,
            CancellationToken cancellationToken = new())
        {
            var content = new StringContent(JsonUtils.Serialize(input), Encoding.UTF8, MediaTypeNames.Application.Json);
            var response = await _httpClient.PostAsync("userRoles", content, cancellationToken);

            return ApiResponse.CreateApiResponse<UserRoleDto>(response);
        }

        public async Task<ApiResponse<UserRoleDto>> UserRoleModifyAsync(Guid id, UserRoleModifyInputDto input,
            CancellationToken cancellationToken = new())
        {
            var content = new StringContent(JsonUtils.Serialize(input), Encoding.UTF8, MediaTypeNames.Application.Json);
            var response = await _httpClient.PatchAsync($"userRoles/{id}", content, cancellationToken);

            return ApiResponse.CreateApiResponse<UserRoleDto>(response);
        }

        public async Task<ApiResponse<UserRoleDto>> UserRoleDeleteAsync(Guid id, CancellationToken cancellationToken = new())
        {
            var response = await _httpClient.DeleteAsync($"userRoles{id}", cancellationToken);

            return ApiResponse.CreateApiResponse<UserRoleDto>(response);
        }
    }
}