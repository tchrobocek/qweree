using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Authentication.AdminSdk.Identity.Clients;
using Qweree.Authentication.WebApi.Domain.Authorization.Roles;
using Qweree.Authentication.WebApi.Domain.Identity;
using Qweree.Mongo.Exception;
using Client = Qweree.Authentication.WebApi.Domain.Identity.Client;
using SdkClient = Qweree.Authentication.AdminSdk.Identity.Clients.Client;
using SdkUser = Qweree.Authentication.AdminSdk.Identity.Users.User;
using SdkUserRole = Qweree.Authentication.AdminSdk.Authorization.Roles.UserRole;
using SdkClientRole = Qweree.Authentication.AdminSdk.Authorization.Roles.ClientRole;

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

        public async Task<SdkUser> MapUserAsync(User user, CancellationToken cancellationToken = new())
        {
            var roles = new List<UserRole>();
            foreach (var role in user.Roles)
            {
                try
                {
                    roles.Add(await _userRoleRepository.GetAsync(role, cancellationToken));
                }
                catch (DocumentNotFoundException)
                {}
            }
            return new(user.Id, user.Username, user.FullName, user.ContactEmail, roles.Select(r=>r.Key).ToImmutableArray(), user.CreatedAt, user.ModifiedAt);
        }

        public async Task<SdkClient> MapClientAsync(Client client, CancellationToken cancellationToken = new())
        {
            var owner = await _userRepository.GetAsync(client.OwnerId, cancellationToken);
            return new(client.Id, client.ClientId, client.ApplicationName, client.Origin, await MapUserAsync(owner, cancellationToken), client.CreatedAt, client.ModifiedAt);
        }

        public async Task<CreatedClient> MapToCreatedClientAsync(Client client, CancellationToken cancellationToken = new())
        {
            var owner = await _userRepository.GetAsync(client.OwnerId, cancellationToken);
            return new(client.Id, client.ClientId, client.ApplicationName, client.Origin, await MapUserAsync(owner, cancellationToken), client.CreatedAt, client.ModifiedAt);
        }
        public async Task<SdkClientRole> MapClientRoleAsync(ClientRole role, CancellationToken cancellationToken = new())
        {
            return await DoMapClientRoleAsync(0, role, cancellationToken);
        }

        private async Task<SdkClientRole> DoMapClientRoleAsync(int level, ClientRole role, CancellationToken cancellationToken = new())
        {
            var items = new List<SdkClientRole>();

            if (role.IsGroup)
            {
                foreach (var itemId in role.Items)
                {
                    var item = await _clientRoleRepository.GetAsync(itemId, cancellationToken);
                    var sdkItem = await DoMapClientRoleAsync(level + 1, item, cancellationToken);
                    items.Add(sdkItem);
                }
            }

            var effectiveRoles = ImmutableArray<string>.Empty;

            if (level == 0)
            {
                var sdkRole = new SdkClientRole(role.Id, role.Key, role.Label,
                    role.Description, items.ToImmutableArray(), role.IsGroup, role.CreatedAt, role.ModifiedAt,
                    effectiveRoles);
                effectiveRoles = ComputeEffectiveRoles(sdkRole)
                    .Distinct()
                    .ToImmutableArray();
            }

            return new SdkClientRole(role.Id, role.Key, role.Label, role.Description, items.ToImmutableArray(),
                role.IsGroup, role.CreatedAt, role.ModifiedAt, effectiveRoles);
        }

        private IEnumerable<string> ComputeEffectiveRoles(SdkClientRole clientRole)
        {
            if (clientRole.IsGroup)
            {
                foreach (var item in clientRole.Items.SelectMany(ComputeEffectiveRoles))
                    yield return item;
            }

            yield return clientRole.Key;
        }

        public async Task<SdkUserRole> MapUserRoleAsync(UserRole role, CancellationToken cancellationToken = new())
        {
            return await DoMapUserRoleAsync(0, role, cancellationToken);
        }

        private async Task<SdkUserRole> DoMapUserRoleAsync(int level, UserRole role, CancellationToken cancellationToken = new())
        {
            var items = new List<SdkUserRole>();

            if (role.IsGroup)
            {
                foreach (var itemId in role.Items)
                {
                    var item = await _userRoleRepository.GetAsync(itemId, cancellationToken);
                    var sdkItem = await DoMapUserRoleAsync(level + 1, item, cancellationToken);
                    items.Add(sdkItem);
                }
            }

            var effectiveRoles = ImmutableArray<string>.Empty;

            if (level == 0)
            {
                var sdkRole = new SdkUserRole(role.Id, role.Key, role.Label,
                    role.Description, items.ToImmutableArray(), role.IsGroup, role.CreatedAt, role.ModifiedAt,
                    effectiveRoles);
                effectiveRoles = ComputeEffectiveRoles(sdkRole)
                    .Distinct()
                    .ToImmutableArray();
            }

            return new SdkUserRole(role.Id, role.Key, role.Label, role.Description, items.ToImmutableArray(),
                role.IsGroup, role.CreatedAt, role.ModifiedAt, effectiveRoles);
        }

        private IEnumerable<string> ComputeEffectiveRoles(SdkUserRole userRole)
        {
            if (userRole.IsGroup)
            {
                foreach (var item in userRole.Items.SelectMany(ComputeEffectiveRoles))
                    yield return item;
            }

            yield return userRole.Key;
        }
    }
}