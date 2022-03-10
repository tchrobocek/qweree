using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Authentication.AdminSdk.Authorization.Roles;
using Qweree.Authentication.WebApi.Domain.Authorization.Roles;
using Qweree.Authentication.WebApi.Domain.Identity;
using Qweree.Mongo.Exception;
using ClientRole = Qweree.Authentication.WebApi.Domain.Authorization.Roles.ClientRole;
using UserRole = Qweree.Authentication.WebApi.Domain.Authorization.Roles.UserRole;

namespace Qweree.Authentication.WebApi.Domain.Authorization;

public class AuthorizationService
{
    private const int MaxSearchLevel = 5;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IClientRoleRepository _clientRoleRepository;

    public AuthorizationService(IUserRoleRepository userRoleRepository, IClientRoleRepository clientRoleRepository)
    {
        _userRoleRepository = userRoleRepository;
        _clientRoleRepository = clientRoleRepository;
    }

    public async IAsyncEnumerable<Role> GetEffectiveUserRoles(User user,
        [EnumeratorCancellation] CancellationToken cancellationToken = new(),
        bool ignoreNonExisting = true)
    {
        var ids = new List<Guid>();

        foreach (var userRoleId in user.Roles)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await foreach (var item in GetEffectiveUserRoles(0, userRoleId, cancellationToken, ignoreNonExisting)
                               .WithCancellation(cancellationToken))
            {
                cancellationToken.ThrowIfCancellationRequested();
                ids.Add(item);
            }
        }

        foreach (var id in ids.Distinct())
        {
            cancellationToken.ThrowIfCancellationRequested();

            UserRole userRole;

            try
            {
                userRole = await _userRoleRepository.GetAsync(id, cancellationToken);
            }
            catch (DocumentNotFoundException)
            {
                if (ignoreNonExisting)
                    continue;

                throw;
            }

            yield return new Role(userRole.Id, userRole.Key, userRole.Label, userRole.Description);
        }
    }

    public async IAsyncEnumerable<Role> GetEffectiveUserRoles(Client client,
        [EnumeratorCancellation] CancellationToken cancellationToken = new(),
        bool ignoreNonExisting = true)
    {
        var ids = new List<Guid>();

        foreach (var userRoleId in client.UserRoles)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await foreach (var item in GetEffectiveUserRoles(0, userRoleId, cancellationToken, ignoreNonExisting)
                               .WithCancellation(cancellationToken))
            {
                cancellationToken.ThrowIfCancellationRequested();
                ids.Add(item);
            }
        }

        foreach (var id in ids.Distinct())
        {
            cancellationToken.ThrowIfCancellationRequested();

            UserRole userRole;

            try
            {
                userRole = await _userRoleRepository.GetAsync(id, cancellationToken);
            }
            catch (DocumentNotFoundException)
            {
                if (ignoreNonExisting)
                    continue;

                throw;
            }

            yield return new Role(userRole.Id, userRole.Key, userRole.Label, userRole.Description);
        }
    }

    public async IAsyncEnumerable<Role> GetEffectiveClientRoles(Client client,
        [EnumeratorCancellation] CancellationToken cancellationToken = new(),
        bool ignoreNonExisting = true)
    {
        var ids = new List<Guid>();

        foreach (var clientRoleId in client.ClientRoles)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await foreach (var item in GetEffectiveClientRoles(0, clientRoleId, cancellationToken, ignoreNonExisting)
                               .WithCancellation(cancellationToken))
            {
                cancellationToken.ThrowIfCancellationRequested();
                ids.Add(item);
            }
        }

        foreach (var id in ids.Distinct())
        {
            cancellationToken.ThrowIfCancellationRequested();

            ClientRole clientRole;

            try
            {
                clientRole = await _clientRoleRepository.GetAsync(id, cancellationToken);
            }
            catch (DocumentNotFoundException)
            {
                if (ignoreNonExisting)
                    continue;

                throw;
            }

            yield return new Role(clientRole.Id, clientRole.Key, clientRole.Label, clientRole.Description);
        }
    }

    // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
    public async IAsyncEnumerable<Role> GetEffectiveUserRoles(UserRole userRole,
        [EnumeratorCancellation] CancellationToken cancellationToken = new(),
        bool ignoreNonExisting = true)
    {
        var ids = new List<Guid>();

        cancellationToken.ThrowIfCancellationRequested();

        await foreach (var item in GetEffectiveUserRoles(0, userRole.Id, cancellationToken, ignoreNonExisting)
                           .WithCancellation(cancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ids.Add(item);
        }

        foreach (var id in ids.Distinct())
        {
            cancellationToken.ThrowIfCancellationRequested();

            UserRole item;

            try
            {
                item = await _userRoleRepository.GetAsync(id, cancellationToken);
            }
            catch (DocumentNotFoundException)
            {
                if (ignoreNonExisting)
                    continue;

                throw;
            }

            yield return new Role(item.Id, item.Key, item.Label, item.Description);
        }
    }

    // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
    private async IAsyncEnumerable<Guid> GetEffectiveUserRoles(int level, Guid userRoleId,
        [EnumeratorCancellation] CancellationToken cancellationToken = new(),
        bool ignoreNonExisting = true)
    {
        if (level > MaxSearchLevel)
            throw new InvalidOperationException($"Max hierarchical level for levels is set to {MaxSearchLevel}.");

        UserRole userRole;

        try
        {
            userRole = await _userRoleRepository.GetAsync(userRoleId, cancellationToken);
        }
        catch (DocumentNotFoundException)
        {
            if (ignoreNonExisting)
                yield break;

            throw;
        }

        yield return userRole.Id;

        foreach (var item in userRole.Items)
        {
            await foreach (var roleId in GetEffectiveUserRoles(level + 1, item, cancellationToken,
                                   ignoreNonExisting)
                               .WithCancellation(cancellationToken))
            {
                yield return roleId;
            }
        }
    }

    // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
    private async IAsyncEnumerable<Guid> GetEffectiveClientRoles(int level, Guid clientRoleId,
        [EnumeratorCancellation] CancellationToken cancellationToken = new(),
        bool ignoreNonExisting = true)
    {
        if (level > MaxSearchLevel)
            throw new InvalidOperationException($"Max hierarchical level for levels is set to {MaxSearchLevel}.");

        ClientRole clientRole;

        try
        {
            clientRole = await _clientRoleRepository.GetAsync(clientRoleId, cancellationToken);
        }
        catch (DocumentNotFoundException)
        {
            if (ignoreNonExisting)
                yield break;

            throw;
        }

        yield return clientRole.Id;

        foreach (var item in clientRole.Items)
        {
            await foreach (var roleId in GetEffectiveClientRoles(level + 1, item, cancellationToken,
                                   ignoreNonExisting)
                               .WithCancellation(cancellationToken))
            {
                yield return roleId;
            }
        }
    }
}