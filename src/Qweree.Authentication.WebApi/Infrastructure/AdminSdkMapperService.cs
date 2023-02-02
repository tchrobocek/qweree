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
using Qweree.Authentication.WebApi.Domain.Identity.UserInvitation;
using Qweree.Mongo.Exception;
using Client = Qweree.Authentication.WebApi.Domain.Identity.Client;
using ClientRole = Qweree.Authentication.WebApi.Domain.Authorization.Roles.ClientRole;
using ClientRoleCreateInput = Qweree.Authentication.WebApi.Domain.Authorization.Roles.ClientRoleCreateInput;
using ClientRoleModifyInput = Qweree.Authentication.WebApi.Domain.Authorization.Roles.ClientRoleModifyInput;
using RolesCollection = Qweree.Authentication.WebApi.Domain.Identity.RolesCollection;
using User = Qweree.Authentication.WebApi.Domain.Identity.User;
using UserProperty = Qweree.Authentication.WebApi.Domain.Identity.UserProperty;
using UserRole = Qweree.Authentication.WebApi.Domain.Authorization.Roles.UserRole;
using SdkUser = Qweree.Authentication.AdminSdk.Identity.Users.User;
using SdkClient = Qweree.Authentication.AdminSdk.Identity.Clients.Client;
using SdkClientRole = Qweree.Authentication.AdminSdk.Authorization.Roles.ClientRole;
using SdkUserRole = Qweree.Authentication.AdminSdk.Authorization.Roles.UserRole;
using SdkUserProperty = Qweree.Authentication.AdminSdk.Identity.Users.UserProperty;
using SdkRolesCollection = Qweree.Authentication.AdminSdk.Authorization.Roles.RolesCollection;
using SdkUserInvitationInput = Qweree.Authentication.AdminSdk.Identity.Users.UserInvitation.UserInvitationInput;
using SdkUserRoleCreateInput = Qweree.Authentication.AdminSdk.Authorization.Roles.UserRoleCreateInput;
using SdkUserRoleModifyInput = Qweree.Authentication.AdminSdk.Authorization.Roles.UserRoleModifyInput;
using SdkClientRoleCreateInput = Qweree.Authentication.AdminSdk.Authorization.Roles.ClientRoleCreateInput;
using SdkClientRoleModifyInput = Qweree.Authentication.AdminSdk.Authorization.Roles.ClientRoleModifyInput;
using UserRoleCreateInput = Qweree.Authentication.WebApi.Domain.Authorization.Roles.UserRoleCreateInput;
using UserRoleModifyInput = Qweree.Authentication.WebApi.Domain.Authorization.Roles.UserRoleModifyInput;

namespace Qweree.Authentication.WebApi.Infrastructure;

public class AdminSdkMapperService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IClientRoleRepository _clientRoleRepository;
    private readonly AuthorizationService _authorizationService;

    public AdminSdkMapperService(IUserRepository userRepository, IUserRoleRepository userRoleRepository,
        IClientRoleRepository clientRoleRepository, AuthorizationService authorizationService)
    {
        _userRepository = userRepository;
        _userRoleRepository = userRoleRepository;
        _clientRoleRepository = clientRoleRepository;
        _authorizationService = authorizationService;
    }

    public async Task<SdkUser> ToUserAsync(User user, CancellationToken cancellationToken = new())
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

        return new SdkUser
        {
            Id = user.Id,
            Username = user.Username,
            ContactEmail = user.ContactEmail,
            CreatedAt = user.CreatedAt,
            ModifiedAt = user.ModifiedAt,
            Properties = user.Properties.Select(ToUserProperty).ToArray(),
            Roles = roles.Select(ToRole).ToArray()
        };
    }

    public async Task<CreatedClient> ToCreatedClientAsync(ClientSecretPair clientSecretPair,
        CancellationToken cancellationToken = new())
    {
        var client = clientSecretPair.Client;

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

        return new CreatedClient
        {
            Id = client.Id,
            ClientId = client.ClientId,
            ClientSecret = clientSecretPair.Secret,
            ApplicationName = client.ApplicationName,
            Origin = client.Origin,
            Owner = await ToUserAsync(owner, cancellationToken),
            CreatedAt = client.CreatedAt,
            ModifiedAt = client.ModifiedAt,
            ClientRoles = roles.Select(ToRole).ToArray()
        };
    }

    public async Task<SdkClientRole> ToClientRoleAsync(ClientRole role,
        CancellationToken cancellationToken = new())
    {
        var effectiveRoles = new List<ClientRole>();
        var items = new List<SdkClientRole>();

        await foreach (var effectiveRole in _authorizationService.GetEffectiveClientRoles(role, cancellationToken)
                           .WithCancellation(cancellationToken))
        {
            effectiveRoles.Add(effectiveRole);
        }

        foreach (var roleId in role.Items)
        {
            var clientRole = effectiveRoles.FirstOrDefault(r => r.Id == roleId);
            if (clientRole is null)
                clientRole = await _clientRoleRepository.GetAsync(roleId, cancellationToken);

            items.Add(await ToClientRoleAsync(clientRole, cancellationToken));
        }

        return new SdkClientRole
        {
            Id = role.Id,
            Description = role.Description,
            Key = role.Key,
            Label = role.Label,
            IsGroup = role.IsGroup,
            CreatedAt = role.CreatedAt,
            ModifiedAt = role.ModifiedAt,
            Items = items.ToArray(),
            EffectiveRoles = effectiveRoles.Select(ToRole).ToArray(),
        };
    }


    public async Task<SdkUserRole> ToUserRoleAsync(UserRole role,
        CancellationToken cancellationToken = new())
    {
        var effectiveRoles = new List<UserRole>();
        var items = new List<SdkUserRole>();

        await foreach (var effectiveRole in _authorizationService.GetEffectiveUserRoles(role, cancellationToken)
                           .WithCancellation(cancellationToken))
        {
            effectiveRoles.Add(effectiveRole);
        }

        foreach (var roleId in role.Items)
        {
            var userRole = effectiveRoles.FirstOrDefault(r => r.Id == roleId);
            if (userRole is null)
                userRole = await _userRoleRepository.GetAsync(roleId, cancellationToken);

            items.Add(await ToUserRoleAsync(userRole, cancellationToken));
        }

        return new SdkUserRole
        {
            Id = role.Id,
            Description = role.Description,
            Key = role.Key,
            Label = role.Label,
            IsGroup = role.IsGroup,
            CreatedAt = role.CreatedAt,
            ModifiedAt = role.ModifiedAt,
            Items = items.ToArray(),
            EffectiveRoles = effectiveRoles.Select(ToRole).ToArray(),
        };
    }

    public Role ToRole(UserRole role)
    {
        return new Role
        {
            Id = role.Id,
            Key = role.Key,
            Label = role.Label,
            Description = role.Description,
        };
    }

    public Role ToRole(ClientRole role)
    {
        return new Role
        {
            Id = role.Id,
            Key = role.Key,
            Label = role.Label,
            Description = role.Description
        };
    }

    private SdkUserProperty ToUserProperty(UserProperty userProperty)
    {
        return new SdkUserProperty
        {
            Key = userProperty.Key,
            Value = userProperty.Value
        };
    }

    public SdkRolesCollection ToRolesCollection(RolesCollection roles)
    {
        return new SdkRolesCollection
        {
            UserRoles = roles.UserRoles.Select(ToRole).ToArray(),
            ClientRoles = roles.ClientRoles.Select(ToRole).ToArray()
        };
    }

    public async Task<SdkClient> ToClient(Client client, CancellationToken cancellationToken = new())
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

        return new SdkClient
        {
            Id = client.Id,
            ClientId = client.ClientId,
            ApplicationName = client.ApplicationName,
            Origin = client.Origin,
            Owner = await ToUserAsync(owner, cancellationToken),
            CreatedAt = client.CreatedAt,
            ModifiedAt = client.ModifiedAt,
            ClientRoles = roles.Select(ToRole).ToArray()
        };
    }

    public UserInvitationInput ToUserInvitation(SdkUserInvitationInput userInvitationInput)
    {
        return new UserInvitationInput(userInvitationInput.Username,
            userInvitationInput.FullName, userInvitationInput.ContactEmail,
            userInvitationInput.Roles?.ToImmutableArray());
    }

    public UserRoleCreateInput ToUserRoleCreateInput(SdkUserRoleCreateInput input)
    {
        return new UserRoleCreateInput(input.Id ?? Guid.Empty, input.Key ?? string.Empty, input.Label ?? string.Empty,
            input.Description ?? string.Empty, input.IsGroup ?? false, input.Items?.ToImmutableArray() ??
                                                                       ImmutableArray<Guid>.Empty);
    }

    public UserRoleModifyInput ToUserRoleModifyInput(Guid id, SdkUserRoleModifyInput input)
    {
        return new UserRoleModifyInput(id, input.Label, input.Description, input.IsGroup, input.Items?.ToImmutableArray());
    }

    public ClientRoleCreateInput ToClientRoleCreateInput(SdkClientRoleCreateInput input)
    {
        return new ClientRoleCreateInput(input.Id ?? Guid.Empty, input.Key ?? string.Empty, input.Label ?? string.Empty,
            input.Description ?? string.Empty, input.IsGroup ?? false, input.Items?.ToImmutableArray() ??
                                                                       ImmutableArray<Guid>.Empty);
    }

    public ClientRoleModifyInput ToClientRoleModifyInput(Guid id, SdkClientRoleModifyInput input)
    {
        return new ClientRoleModifyInput(id, input.Label, input.Description, input.IsGroup, input.Items?.ToImmutableArray());
    }
}