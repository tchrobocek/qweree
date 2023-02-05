using System;
using System.Collections.Immutable;

namespace Qweree.Authentication.WebApi.Domain.Identity;

public interface IAccessDefinitionInput
{
}
public class ClientCredentialsDefinitionInput : IAccessDefinitionInput
{
    public ClientCredentialsDefinitionInput(ImmutableArray<Guid> roles)
    {
        Roles = roles;
    }

    public ImmutableArray<Guid> Roles { get; }
}
public class PasswordDefinitionInput : IAccessDefinitionInput
{
}