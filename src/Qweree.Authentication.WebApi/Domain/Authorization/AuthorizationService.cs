using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Authentication.WebApi.Domain.Authorization.Roles;
using Qweree.Authentication.WebApi.Domain.Identity;
using Qweree.Mongo.Exception;

namespace Qweree.Authentication.WebApi.Domain.Authorization;

public class AuthorizationService
{
    private const int MaxSearchLevel = 5;
    private readonly IUserRoleRepository _userRoleRepository;

    public AuthorizationService(IUserRoleRepository userRoleRepository)
    {
        _userRoleRepository = userRoleRepository;
    }

    public async IAsyncEnumerable<UserRole> GetEffectiveUserRoles(User user,
        [EnumeratorCancellation] CancellationToken cancellationToken = new(),
        bool ignoreNonExisting = true)
    {
        foreach (var userRoleId in user.Roles)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await foreach (var item in GetEffectiveUserRoles(0, userRoleId, cancellationToken, ignoreNonExisting)
                               .WithCancellation(cancellationToken))
            {
                yield return item;
            }
        }
    }

    public async IAsyncEnumerable<UserRole> GetEffectiveUserRoles(Client client,
        [EnumeratorCancellation] CancellationToken cancellationToken = new(),
        bool ignoreNonExisting = true)
    {
        foreach (var userRoleId in client.UserRoles)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await foreach (var item in GetEffectiveUserRoles(0, userRoleId, cancellationToken, ignoreNonExisting)
                               .WithCancellation(cancellationToken))
            {
                cancellationToken.ThrowIfCancellationRequested();
                yield return item;
            }
        }
    }

    // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
    public async IAsyncEnumerable<UserRole> GetEffectiveUserRoles(UserRole userRole,
        [EnumeratorCancellation] CancellationToken cancellationToken = new(),
        bool ignoreNonExisting = true)
    {
        cancellationToken.ThrowIfCancellationRequested();

        await foreach (var item in GetEffectiveUserRoles(0, userRole.Id, cancellationToken, ignoreNonExisting)
                           .WithCancellation(cancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return item;
        }
    }

    // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
    private async IAsyncEnumerable<UserRole> GetEffectiveUserRoles(int level, Guid userRoleId,
        [EnumeratorCancellation] CancellationToken cancellationToken = new(),
        bool ignoreNonExisting = true)
    {
        if (level > MaxSearchLevel)
            throw new InvalidOperationException($"Max hierarchical level for roles is set to {MaxSearchLevel}.");

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

        yield return userRole;

        foreach (var item in userRole.Items)
        {
            await foreach (var role in GetEffectiveUserRoles(level + 1, item, cancellationToken,
                                   ignoreNonExisting)
                               .WithCancellation(cancellationToken))
            {
                yield return role;
            }
        }
    }
}