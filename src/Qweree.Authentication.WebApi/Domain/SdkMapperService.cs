using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Authentication.AdminSdk.Authorization.Roles;
using Qweree.Authentication.AdminSdk.Identity.Clients;
using Qweree.Authentication.WebApi.Domain.Authorization;
using Qweree.Authentication.WebApi.Domain.Authorization.Roles;
using Qweree.Authentication.WebApi.Domain.Identity;
using Qweree.Mongo.Exception;
using Client = Qweree.Authentication.WebApi.Domain.Identity.Client;
using ClientRole = Qweree.Authentication.WebApi.Domain.Authorization.Roles.ClientRole;
using SdkClient = Qweree.Authentication.AdminSdk.Identity.Clients.Client;
using SdkUser = Qweree.Authentication.AdminSdk.Identity.Users.User;
using SdkUserRole = Qweree.Authentication.AdminSdk.Authorization.Roles.UserRole;
using SdkClientRole = Qweree.Authentication.AdminSdk.Authorization.Roles.ClientRole;
using SdkUserProperty = Qweree.Authentication.Sdk.Users.UserProperty;
using UserRole = Qweree.Authentication.WebApi.Domain.Authorization.Roles.UserRole;

namespace Qweree.Authentication.WebApi.Domain;

public class SdkMapperService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IClientRoleRepository _clientRoleRepository;
    private readonly AuthorizationService _authorizationService;

    public SdkMapperService(IUserRepository userRepository, IUserRoleRepository userRoleRepository,
        IClientRoleRepository clientRoleRepository, AuthorizationService authorizationService)
    {
        _userRepository = userRepository;
        _userRoleRepository = userRoleRepository;
        _clientRoleRepository = clientRoleRepository;
        _authorizationService = authorizationService;
    }

    public async Task<SdkUser> UserMapAsync(User user, CancellationToken cancellationToken = new())
    {
        var roles = new List<UserRole>();
        foreach (var role in user.Roles)
        {
            UserRole userRole;
            try
            {
                userRole = await _userRoleRepository.GetAsync(role, cancellationToken);
            }
            catch (DocumentNotFoundException)
            {
                continue;
            }

            roles.Add(userRole);
        }

        return new SdkUser(user.Id, user.Username, user.ContactEmail, user.Properties.Select(FromUserProperty)
            .ToImmutableArray(), roles.Select(FromUserRole).ToImmutableArray(), user.CreatedAt, user.ModifiedAt);
    }

    public async Task<SdkClient> ClientMapAsync(Client client, CancellationToken cancellationToken = new())
    {
        var clientRoles = new List<ClientRole>();
        foreach (var role in client.ClientRoles)
        {
            try
            {
                clientRoles.Add(await _clientRoleRepository.GetAsync(role, cancellationToken));
            }
            catch (DocumentNotFoundException)
            {
            }
        }

        var userRoles = new List<UserRole>();
        foreach (var role in client.UserRoles)
        {
            try
            {
                userRoles.Add(await _userRoleRepository.GetAsync(role, cancellationToken));
            }
            catch (DocumentNotFoundException)
            {
            }
        }

        var owner = await _userRepository.GetAsync(client.OwnerId, cancellationToken);
        return new SdkClient(client.Id, client.ClientId, client.ApplicationName, client.Origin,
            await UserMapAsync(owner, cancellationToken), clientRoles.Select(FromClientRole).ToImmutableArray(),
             userRoles.Select(FromUserRole).ToImmutableArray(), client.CreatedAt, client.ModifiedAt);
    }

    public async Task<CreatedClient> ClientMapToCreatedClientAsync(Client client,
        CancellationToken cancellationToken = new())
    {
        var roles = new List<ClientRole>();
        foreach (var role in client.ClientRoles)
        {
            try
            {
                roles.Add(await _clientRoleRepository.GetAsync(role, cancellationToken));
            }
            catch (DocumentNotFoundException)
            {
            }
        }

        var owner = await _userRepository.GetAsync(client.OwnerId, cancellationToken);
        return new CreatedClient(client.Id, client.ClientId, client.ClientSecret, client.ApplicationName, client.Origin, await UserMapAsync(owner,
            cancellationToken), client.CreatedAt, client.ModifiedAt, roles.Select(FromClientRole).ToImmutableArray());
    }

    public async Task<SdkClientRole> ClientRoleMapAsync(ClientRole role,
        CancellationToken cancellationToken = new())
    {
        return await DoMapClientRoleAsync(0, role, cancellationToken);
    }

    private async Task<SdkClientRole> DoMapClientRoleAsync(int level, ClientRole role,
        CancellationToken cancellationToken = new())
    {
        var items = new List<SdkClientRole>();

        if (role.IsGroup)
        {
            foreach (var itemId in role.Items)
            {
                ClientRole item;

                try
                {
                    item = await _clientRoleRepository.GetAsync(itemId, cancellationToken);
                }
                catch (Exception)
                {
                    continue;
                }

                var sdkItem = await DoMapClientRoleAsync(level + 1, item, cancellationToken);
                items.Add(sdkItem);
            }
        }

        var effectiveRoles = ImmutableArray<Role>.Empty;

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

    private IEnumerable<Role> ComputeEffectiveRoles(SdkClientRole clientRole)
    {
        if (clientRole.IsGroup)
        {
            foreach (var item in clientRole.Items.SelectMany(ComputeEffectiveRoles))
                yield return new Role(item.Id, item.Key, item.Label, item.Description);
        }

        yield return new Role(clientRole.Id, clientRole.Key, clientRole.Label, clientRole.Description);
    }

    public async Task<SdkUserRole> UserRoleMapAsync(UserRole role, CancellationToken cancellationToken = new())
    {
        return await DoMapUserRoleAsync(0, role, cancellationToken);
    }

    private async Task<SdkUserRole> DoMapUserRoleAsync(int level, UserRole role,
        CancellationToken cancellationToken = new())
    {
        var items = new List<SdkUserRole>();

        if (role.IsGroup)
        {
            foreach (var itemId in role.Items)
            {
                UserRole item;

                try
                {
                    item = await _userRoleRepository.GetAsync(itemId, cancellationToken);
                }
                catch (Exception)
                {
                    continue;
                }

                var sdkItem = await DoMapUserRoleAsync(level + 1, item, cancellationToken);
                items.Add(sdkItem);
            }
        }

        var effectiveRoles = new List<Role>();

        if (level == 0)
        {
            await foreach (var effectiveRole in _authorizationService.GetEffectiveUserRoles(role, cancellationToken)
                               .WithCancellation(cancellationToken))
            {
                effectiveRoles.Add(effectiveRole);
            }
        }

        return new SdkUserRole(role.Id, role.Key, role.Label, role.Description, items.ToImmutableArray(),
            role.IsGroup, role.CreatedAt, role.ModifiedAt, effectiveRoles.ToImmutableArray());
    }

    private Role FromUserRole(UserRole role)
    {
        return new Role(role.Id, role.Key, role.Label, role.Description);
    }

    private Role FromClientRole(ClientRole role)
    {
        return new Role(role.Id, role.Key, role.Label, role.Description);
    }

    private SdkUserProperty FromUserProperty(UserProperty userProperty)
    {
        return new SdkUserProperty(userProperty.Key, userProperty.Value);
    }
}