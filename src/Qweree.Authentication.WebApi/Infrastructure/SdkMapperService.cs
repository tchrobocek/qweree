using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Authentication.AdminSdk.Authorization.Roles;
using Qweree.Authentication.AdminSdk.Identity.Clients;
using Qweree.Authentication.AdminSdk.Identity.Users;
using Qweree.Authentication.WebApi.Domain.Authorization;
using Qweree.Authentication.WebApi.Domain.Authorization.Roles;
using Qweree.Authentication.WebApi.Domain.Identity;
using Qweree.Mongo.Exception;
using ClientRole = Qweree.Authentication.WebApi.Domain.Authorization.Roles.ClientRole;
using User = Qweree.Authentication.WebApi.Domain.Identity.User;
using UserProperty = Qweree.Authentication.WebApi.Domain.Identity.UserProperty;
using UserRole = Qweree.Authentication.WebApi.Domain.Authorization.Roles.UserRole;

namespace Qweree.Authentication.WebApi.Infrastructure;

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

    public async Task<UserDto> MapToUserAsync(User user, CancellationToken cancellationToken = new())
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

        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            ContactEmail = user.ContactEmail,
            CreatedAt = user.CreatedAt,
            ModifiedAt = user.ModifiedAt,
            Properties = user.Properties.Select(FromUserPropertyDto).ToArray(),
            Roles = roles.Select(MapToRole).ToArray()
        };
    }

    public async Task<CreatedClientDto> MapToCreatedClientAsync(ClientSecretPair clientSecretPair,
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

        return new CreatedClientDto
        {
            Id = client.Id,
            ClientId = client.ClientId,
            ClientSecret = clientSecretPair.Secret,
            ApplicationName = client.ApplicationName,
            Origin = client.Origin,
            Owner = await MapToUserAsync(owner, cancellationToken),
            CreatedAt = client.CreatedAt,
            ModifiedAt = client.ModifiedAt,
            ClientRoles = roles.Select(MapToRole).ToArray()
        };
    }

    public async Task<ClientRoleDto> MapToClientRoleAsync(ClientRole role,
        CancellationToken cancellationToken = new())
    {
        var effectiveRoles = new List<ClientRole>();
        var items = new List<ClientRoleDto>();

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

            items.Add(await MapToClientRoleAsync(clientRole, cancellationToken));
        }

        return new ClientRoleDto
        {
            Id = role.Id,
            Description = role.Description,
            Key = role.Key,
            Label = role.Label,
            IsGroup = role.IsGroup,
            CreatedAt = role.CreatedAt,
            ModifiedAt = role.ModifiedAt,
            Items = items.ToArray(),
            EffectiveRoles = effectiveRoles.Select(MapToRole).ToArray(),
        };
    }


    public async Task<UserRoleDto> MapToUserRoleAsync(UserRole role,
        CancellationToken cancellationToken = new())
    {
        var effectiveRoles = new List<UserRole>();
        var items = new List<UserRoleDto>();

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

            items.Add(await MapToUserRoleAsync(userRole, cancellationToken));
        }

        return new UserRoleDto
        {
            Id = role.Id,
            Description = role.Description,
            Key = role.Key,
            Label = role.Label,
            IsGroup = role.IsGroup,
            CreatedAt = role.CreatedAt,
            ModifiedAt = role.ModifiedAt,
            Items = items.ToArray(),
            EffectiveRoles = effectiveRoles.Select(MapToRole).ToArray(),
        };
    }

    public RoleDto MapToRole(UserRole role)
    {
        return new RoleDto
        {
            Id = role.Id,
            Key = role.Key,
            Label = role.Label,
            Description = role.Description,
        };
    }

    public RoleDto MapToRole(ClientRole role)
    {
        return new RoleDto
        {
            Id = role.Id,
            Key = role.Key,
            Label = role.Label,
            Description = role.Description
        };
    }

    private UserPropertyDto FromUserPropertyDto(UserProperty userProperty)
    {
        return new UserPropertyDto
        {
            Key = userProperty.Key,
            Value = userProperty.Value
        };
    }

    public RolesCollectionDto MapToRolesCollection(RolesCollection roles)
    {
        return new RolesCollectionDto
        {
            UserRoles = roles.UserRoles.Select(MapToRole).ToArray(),
            ClientRoles = roles.ClientRoles.Select(MapToRole).ToArray()
        };
    }

    public async Task<ClientDto> MapToClient(Client client, CancellationToken cancellationToken = new())
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

        return new ClientDto
        {
            Id = client.Id,
            ClientId = client.ClientId,
            ApplicationName = client.ApplicationName,
            Origin = client.Origin,
            Owner = await MapToUserAsync(owner, cancellationToken),
            CreatedAt = client.CreatedAt,
            ModifiedAt = client.ModifiedAt,
            ClientRoles = roles.Select(MapToRole).ToArray()
        };
    }
}