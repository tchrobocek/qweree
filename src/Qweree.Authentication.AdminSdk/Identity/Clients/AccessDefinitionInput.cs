using System;

namespace Qweree.Authentication.AdminSdk.Identity.Clients;

public interface IAccessDefinitionInput
{
    string TypeName { get; }
}

public class ClientCredentialsAccessDefinitionInput : IAccessDefinitionInput
{
    public string TypeName => "client_credentials";
    public Guid[]? Roles { get; set; }

}

public class PasswordAccessDefinitionInput : IAccessDefinitionInput
{
    public string TypeName => "password";
}

public class ImplicitAccessDefinitionInput : IAccessDefinitionInput
{
    public string TypeName => "implicit";
    public string? RedirectUri { get; set; }

}

public class AuthorizationCodeAccessDefinitionInput : IAccessDefinitionInput
{
    public string TypeName => "authorizationCode";
    public string? RedirectUri { get; set; }

}