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

public class ImplicitAccessDefinitionInput : IAccessDefinitionInput
{
    public ImplicitAccessDefinitionInput(string redirectUri)
    {
        RedirectUri = redirectUri;
    }

    public string RedirectUri { get; }
}

public class AuthorizationCodeAccessDefinitionInput : IAccessDefinitionInput
{
    public AuthorizationCodeAccessDefinitionInput(string redirectUri)
    {
        RedirectUri = redirectUri;
    }

    public string RedirectUri { get; }
}