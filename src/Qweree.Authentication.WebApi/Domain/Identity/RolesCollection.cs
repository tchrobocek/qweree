using System.Collections.Immutable;
using Qweree.Authentication.WebApi.Domain.Authorization.Roles;

namespace Qweree.Authentication.WebApi.Domain.Identity;

public class RolesCollection
{
    public RolesCollection(ImmutableArray<UserRole> userRoles)
    {
        UserRoles = userRoles;
    }

    public ImmutableArray<UserRole> UserRoles { get; }
}