using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Authentication.AdminSdk.Identity.Clients;
using Qweree.Authentication.WebApi.Domain.Authorization;
using Qweree.Authentication.WebApi.Domain.Identity;
using Client = Qweree.Authentication.WebApi.Domain.Identity.Client;
using SdkClient = Qweree.Authentication.AdminSdk.Identity.Clients.Client;
using SdkUser = Qweree.Authentication.AdminSdk.Identity.Users.User;
using SdkUserRole = Qweree.Authentication.AdminSdk.Authorization.UserRole;
using SdkClientRole = Qweree.Authentication.AdminSdk.Authorization.ClientRole;

namespace Qweree.Authentication.WebApi.Domain
{
    public class SdkMapperService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IClientRoleRepository _clientRoleRepository;

        public SdkMapperService(IUserRepository userRepository, IUserRoleRepository userRoleRepository, IClientRoleRepository clientRoleRepository)
        {
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
            _clientRoleRepository = clientRoleRepository;
        }

        public SdkUser MapUser(User user)
        {
            return new(user.Id, user.Username, user.FullName, user.ContactEmail, user.Roles, user.CreatedAt, user.ModifiedAt);
        }

        public async Task<SdkClient> MapClientAsync(Client client, CancellationToken cancellationToken = new())
        {
            var owner = await _userRepository.GetAsync(client.OwnerId, cancellationToken);
            return new(client.Id, client.ClientId, client.ApplicationName, client.Origin, MapUser(owner), client.CreatedAt, client.ModifiedAt);
        }

        public async Task<CreatedClient> MapToCreatedClientAsync(Client client, CancellationToken cancellationToken = new())
        {
            var owner = await _userRepository.GetAsync(client.OwnerId, cancellationToken);
            return new(client.Id, client.ClientId, client.ApplicationName, client.Origin, MapUser(owner), client.CreatedAt, client.ModifiedAt);
        }

        public async Task<SdkUserRole> MapUserRoleAsync(UserRole role, CancellationToken cancellationToken = new())
        {
            var items = new List<SdkUserRole>();

            if (role.IsGroup)
            {
                foreach (var itemId in role.Items)
                {
                    var item = await _userRoleRepository.GetAsync(itemId, cancellationToken);
                    var sdkItem = await MapUserRoleAsync(item, cancellationToken);

                    items.Add(sdkItem);
                }
            }

            return new SdkUserRole(role.Id, role.Key, role.Label, role.Description, items.ToImmutableArray(),
                role.IsGroup, role.CreatedAt, role.ModifiedAt, ImmutableArray<SdkUserRole>.Empty);
        }

        public async Task<SdkClientRole> MapClientRoleAsync(ClientRole role, CancellationToken cancellationToken = new())
        {
            var items = new List<SdkClientRole>();

            if (role.IsGroup)
            {
                foreach (var itemId in role.Items)
                {
                    var item = await _clientRoleRepository.GetAsync(itemId, cancellationToken);
                    var sdkItem = await MapClientRoleAsync(item, cancellationToken);

                    items.Add(sdkItem);
                }
            }

            return new SdkClientRole(role.Id, role.Key, role.Label, role.Description, items.ToImmutableArray(),
                role.IsGroup, role.CreatedAt, role.ModifiedAt, ImmutableArray<SdkClientRole>.Empty);
        }
    }
}