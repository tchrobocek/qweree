using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Authentication.WebApi.Domain.Authorization.Roles;
using Qweree.Mongo.Exception;

namespace Qweree.Authentication.WebApi.Domain.Authorization;

public class AuthorizationService
{
    private const int MaxSearchLevel = 5;
    private readonly IRoleRepository _roleRepository;

    public AuthorizationService(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async IAsyncEnumerable<Role> GetEffectiveRoles(IEnumerable<Guid> roles,
        [EnumeratorCancellation] CancellationToken cancellationToken = new(),
        bool ignoreNonExisting = true)
    {
        foreach (var roleId in roles)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await foreach (var item in GetEffectiveRoles(0, roleId, cancellationToken, ignoreNonExisting)
                               .WithCancellation(cancellationToken))
            {
                cancellationToken.ThrowIfCancellationRequested();
                yield return item;
            }
        }
    }

    // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
    public async IAsyncEnumerable<Role> GetEffectiveRoles(Role role,
        [EnumeratorCancellation] CancellationToken cancellationToken = new(),
        bool ignoreNonExisting = true)
    {
        cancellationToken.ThrowIfCancellationRequested();

        await foreach (var item in GetEffectiveRoles(0, role.Id, cancellationToken, ignoreNonExisting)
                           .WithCancellation(cancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return item;
        }
    }

    // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
    private async IAsyncEnumerable<Role> GetEffectiveRoles(int level, Guid roleId,
        [EnumeratorCancellation] CancellationToken cancellationToken = new(),
        bool ignoreNonExisting = true)
    {
        if (level > MaxSearchLevel)
            throw new InvalidOperationException($"Max hierarchical level for roles is set to {MaxSearchLevel}.");

        Role role;

        try
        {
            role = await _roleRepository.GetAsync(roleId, cancellationToken);
        }
        catch (DocumentNotFoundException)
        {
            if (ignoreNonExisting)
                yield break;

            throw;
        }

        yield return role;

        foreach (var item in role.Items)
        {
            await foreach (var child in GetEffectiveRoles(level + 1, item, cancellationToken,
                                   ignoreNonExisting)
                               .WithCancellation(cancellationToken))
            {
                yield return child;
            }
        }
    }
}