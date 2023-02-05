using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Authentication.AdminSdk.Identity.Clients;
using Qweree.Authentication.WebApi.Domain.Authorization;
using Qweree.Authentication.WebApi.Domain.Authorization.Roles;
using Qweree.Authentication.WebApi.Domain.Identity;
using Qweree.Authentication.WebApi.Domain.Identity.UserInvitation;
using Qweree.Authentication.WebApi.Domain.Session;
using Qweree.Mongo.Exception;
using Client = Qweree.Authentication.WebApi.Domain.Identity.Client;
using ClientCredentialsAccessDefinition = Qweree.Authentication.WebApi.Domain.Identity.ClientCredentialsAccessDefinition;
using RolesCollection = Qweree.Authentication.WebApi.Domain.Identity.RolesCollection;
using User = Qweree.Authentication.WebApi.Domain.Identity.User;
using UserProperty = Qweree.Authentication.WebApi.Domain.Identity.UserProperty;
using Role = Qweree.Authentication.WebApi.Domain.Authorization.Roles.Role;
using SdkUser = Qweree.Authentication.AdminSdk.Identity.Users.User;
using SdkClient = Qweree.Authentication.AdminSdk.Identity.Clients.Client;
using SdkRole = Qweree.Authentication.AdminSdk.Authorization.Roles.Role;
using SdkUserProperty = Qweree.Authentication.AdminSdk.Identity.Users.UserProperty;
using SdkRolesCollection = Qweree.Authentication.AdminSdk.Authorization.Roles.RolesCollection;
using SdkUserInvitationInput = Qweree.Authentication.AdminSdk.Identity.Users.UserInvitation.UserInvitationInput;
using SdkRoleCreateInput = Qweree.Authentication.AdminSdk.Authorization.Roles.RoleCreateInput;
using SdkRoleModifyInput = Qweree.Authentication.AdminSdk.Authorization.Roles.RoleModifyInput;
using SdkSessionInfo = Qweree.Authentication.AdminSdk.Session.SessionInfo;
using SdkUserAgentInfo = Qweree.Authentication.AdminSdk.Session.UserAgentInfo;
using ISdkClientInfo = Qweree.Authentication.AdminSdk.Session.IClientInfo;
using PasswordAccessDefinition = Qweree.Authentication.WebApi.Domain.Identity.PasswordAccessDefinition;
using SdkBotClientInfo = Qweree.Authentication.AdminSdk.Session.BotClientInfo;
using SdkBrowserClientInfo = Qweree.Authentication.AdminSdk.Session.BrowserClientInfo;
using SdkOperationSystemInfo = Qweree.Authentication.AdminSdk.Session.OperationSystemInfo;
using SdkIAccessDefinition = Qweree.Authentication.AdminSdk.Identity.Clients.IAccessDefinition;
using SdkPasswordAccessDefinition = Qweree.Authentication.AdminSdk.Identity.Clients.PasswordAccessDefinition;
using SdkClientCredentialsAccessDefinition = Qweree.Authentication.AdminSdk.Identity.Clients.ClientCredentialsAccessDefinition;
using SdkIAccessDefinitionInput = Qweree.Authentication.AdminSdk.Identity.Clients.IAccessDefinitionInput;
using SdkPasswordAccessDefinitionInput = Qweree.Authentication.AdminSdk.Identity.Clients.PasswordAccessDefinitionInput;
using SdkClientCredentialsAccessDefinitionInput = Qweree.Authentication.AdminSdk.Identity.Clients.ClientCredentialsAccessDefinitionInput;

namespace Qweree.Authentication.WebApi.Infrastructure;

public class AdminSdkMapperService
{
    private readonly IClientRepository _clientRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly AuthorizationService _authorizationService;

    public AdminSdkMapperService(IUserRepository userRepository, IRoleRepository roleRepository,
        AuthorizationService authorizationService, IClientRepository clientRepository)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _authorizationService = authorizationService;
        _clientRepository = clientRepository;
    }

    public async Task<SdkUser> ToUserAsync(User user, CancellationToken cancellationToken = new())
    {
        var roles = new List<Role>();
        foreach (var userRole in user.Roles)
        {
            Role role;
            try
            {
                role = await _roleRepository.GetAsync(userRole, cancellationToken);
            }
            catch (DocumentNotFoundException)
            {
                continue;
            }

            roles.Add(role);
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

    public async Task<ClientWithSecret> ToClientWithSecretAsync(ClientSecretPair clientSecretPair,
        CancellationToken cancellationToken = new())
    {
        var client = await ToClientAsync(clientSecretPair.Client, cancellationToken);
        return new ClientWithSecret
        {
            Id = client.Id,
            ApplicationName = client.ApplicationName,
            Origin = client.Origin,
            ClientId = client.ClientId,
            ClientSecret = clientSecretPair.Secret,
            AccessDefinitions = client.AccessDefinitions,
            Owner = client.Owner,
            CreatedAt = client.CreatedAt,
            ModifiedAt = client.ModifiedAt,
            Roles = client.Roles
        };
    }

    public async Task<SdkRole> ToRoleAsync(Role role,
        CancellationToken cancellationToken = new())
    {
        var effectiveRoles = new List<Role>();
        var items = new List<SdkRole>();

        await foreach (var effectiveRole in _authorizationService.GetEffectiveRoles(role, cancellationToken)
                           .WithCancellation(cancellationToken))
        {
            effectiveRoles.Add(effectiveRole);
        }

        foreach (var roleId in role.Items)
        {
            var effectiveRole = effectiveRoles.FirstOrDefault(r => r.Id == roleId);
            if (effectiveRole is null)
                effectiveRole = await _roleRepository.GetAsync(roleId, cancellationToken);

            items.Add(await ToRoleAsync(effectiveRole, cancellationToken));
        }

        return new SdkRole
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

    public SdkRole ToRole(Role role)
    {
        return new SdkRole
        {
            Id = role.Id,
            Key = role.Key,
            Label = role.Label,
            Description = role.Description,
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
            Roles = roles.Roles.Select(ToRole).ToArray(),
        };
    }

    public async Task<SdkClient> ToClientAsync(Client client, CancellationToken cancellationToken = new())
    {
        var roles = new List<Role>();
        foreach (var role in client.Roles)
        {
            try
            {
                roles.Add(await _roleRepository.GetAsync(role, cancellationToken));
            }
            catch (DocumentNotFoundException)
            {
            }
        }

        var accessDefinitions = new List<SdkIAccessDefinition>();
        foreach (var definition in client.AccessDefinitions)
        {
            if (definition is PasswordAccessDefinition)
            {
                accessDefinitions.Add(new SdkPasswordAccessDefinition());
            }

            if (definition is ClientCredentialsAccessDefinition clientCredentials)
            {
                var defRoles = new List<SdkRole>();
                foreach (var roleId in clientCredentials.Roles)
                {
                    var role = await _roleRepository.GetAsync(roleId, cancellationToken);
                    defRoles.Add(await ToRoleAsync(role, cancellationToken));
                }

                accessDefinitions.Add(new SdkClientCredentialsAccessDefinition
                {
                    Roles = defRoles.ToArray()
                });
            }

            throw new ArgumentOutOfRangeException(nameof(definition));
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
            Roles = roles.Select(ToRole).ToArray(),
            AccessDefinitions = accessDefinitions.ToArray()
        };
    }

    public UserInvitationInput ToUserInvitation(SdkUserInvitationInput userInvitationInput)
    {
        return new UserInvitationInput(userInvitationInput.Username,
            userInvitationInput.FullName, userInvitationInput.ContactEmail,
            userInvitationInput.Roles?.ToImmutableArray());
    }

    public RoleCreateInput ToRoleCreateInput(SdkRoleCreateInput input)
    {
        return new RoleCreateInput(input.Id ?? Guid.Empty, input.Key ?? string.Empty, input.Label ?? string.Empty,
            input.Description ?? string.Empty, input.IsGroup ?? false, input.Items?.ToImmutableArray() ??
                                                                       ImmutableArray<Guid>.Empty);
    }

    public RoleModifyInput ToRoleModifyInput(Guid id, SdkRoleModifyInput input)
    {
        return new RoleModifyInput(id, input.Label, input.Description, input.IsGroup, input.Items?.ToImmutableArray());
    }

    public async Task<IEnumerable<SdkSessionInfo>> ToSessionInfosAsync(IEnumerable<SessionInfo> sessionInfos)
    {
        var result = new List<SdkSessionInfo>();

        sessionInfos = sessionInfos.ToArray();

        var clientIds = sessionInfos.Select(i => i.ClientId);
        var userIds = sessionInfos.Where(i => i.UserId is not null)
            .Select(i => i.UserId)
            .Cast<Guid>();

        var clients = (await _clientRepository.GetAsync(clientIds))
            .ToDictionary(c => c.Id);
        var users = (await _userRepository.GetAsync(userIds))
            .ToDictionary(u => u.Id);

        foreach (var sessionInfo in sessionInfos)
        {
            result.Add(new SdkSessionInfo
            {
                Id = sessionInfo.Id,
                Client = await ToClientAsync(clients[sessionInfo.ClientId]),
                User = sessionInfo.UserId is not null ? await ToUserAsync(users[(Guid)sessionInfo.UserId!]) : null,
                Grant = sessionInfo.Grant.ToString(),
                IpAddress = sessionInfo.IpAddress,
                UserAgent = sessionInfo.UserAgent is not null ? ToUserAgentInfo(sessionInfo.UserAgent) : null,
                CreatedAt = sessionInfo.CreatedAt,
                IssuedAt = sessionInfo.IssuedAt,
                ExpiresAt = sessionInfo.ExpiresAt,
            });
        }

        return result;
    }

    private SdkUserAgentInfo ToUserAgentInfo(UserAgentInfo userAgent)
    {
        return new SdkUserAgentInfo
        {
            Brand = userAgent.Brand,
            Device = userAgent.Device,
            Model = userAgent.Model,
            Client = userAgent.Client is not null ? ToClientInfo(userAgent.Client) : null,
            OperationSystem = userAgent.OperationSystem is not null ? ToOperationSystem(userAgent.OperationSystem) : null
        };
    }

    private ISdkClientInfo ToClientInfo(IClientInfo clientInfo)
    {
        if (clientInfo is BotClientInfo bot)
        {
            return new SdkBotClientInfo
            {
                Name = bot.ClientString
            };
        }

        if (clientInfo is BrowserClientInfo browser)
        {
            return new SdkBrowserClientInfo
            {
                Name = browser.Name,
                Version = browser.Version,
                ShortName = browser.ShortName,
                Engine = browser.Engine,
                EngineVersion = browser.EngineVersion
            };
        }

        throw new ArgumentException($"Convertor for client info of type {clientInfo.GetType()} is not implemented.");
    }

    private SdkOperationSystemInfo ToOperationSystem(OperationSystemInfo os)
    {
        return new SdkOperationSystemInfo
        {
            Name = os.Name,
            Platform = os.Platform,
            Version = os.Version,
            ShortName = os.ShortName
        };
    }
}