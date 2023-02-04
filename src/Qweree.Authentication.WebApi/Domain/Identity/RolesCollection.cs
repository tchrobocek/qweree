using System.Collections.Immutable;
using Qweree.Authentication.WebApi.Domain.Authorization.Roles;

namespace Qweree.Authentication.WebApi.Domain.Identity;

public class RolesCollection
{
    public RolesCollection(ImmutableArray<Role> roles)
    {
        Roles = roles;
    }

    public ImmutableArray<Role> Roles { get; }
}