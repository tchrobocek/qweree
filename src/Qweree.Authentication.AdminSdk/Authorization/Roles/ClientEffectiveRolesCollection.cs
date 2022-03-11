using System.Collections.Immutable;

namespace Qweree.Authentication.AdminSdk.Authorization.Roles;

public class ClientEffectiveRolesCollection
{
    public ClientEffectiveRolesCollection(ImmutableArray<Role> userEffectiveRoles, ImmutableArray<Role> clientEffectiveRoles)
    {
        UserEffectiveRoles = userEffectiveRoles;
        ClientEffectiveRoles = clientEffectiveRoles;
    }

    public ImmutableArray<Role> UserEffectiveRoles { get; }
    public ImmutableArray<Role> ClientEffectiveRoles { get; }
}