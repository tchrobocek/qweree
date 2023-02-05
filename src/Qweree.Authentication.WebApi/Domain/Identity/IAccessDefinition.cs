using System;
using System.Collections.Immutable;
using Qweree.Authentication.WebApi.Domain.Authentication;

namespace Qweree.Authentication.WebApi.Domain.Identity;

public interface IAccessDefinition
{
    GrantType GrantType { get; }
}

public class ClientCredentialsAccessDefinition : IAccessDefinition
{
    public ClientCredentialsAccessDefinition(ImmutableArray<Guid> roles)
    {
        Roles = roles;
    }

    public GrantType GrantType => GrantType.ClientCredentials;
    public ImmutableArray<Guid> Roles { get; }

}

public class PasswordAccessDefinition : IAccessDefinition
{
    public GrantType GrantType => GrantType.Password;
}
