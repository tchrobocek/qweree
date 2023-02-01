using System.Collections.Immutable;
using Qweree.Authentication.WebApi.Domain.Authorization.Roles;

namespace Qweree.Authentication.WebApi.Domain.Identity;

public class RolesCollection
{
    public RolesCollection(ImmutableArray<UserRole> userRoles, ImmutableArray<ClientRole> clientRoles)
    {
        UserRoles = userRoles;
        ClientRoles = clientRoles;
    }

    public ImmutableArray<UserRole> UserRoles { get; }
    public ImmutableArray<ClientRole> ClientRoles { get; }
}