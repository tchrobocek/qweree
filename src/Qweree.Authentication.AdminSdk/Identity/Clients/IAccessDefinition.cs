using System;

namespace Qweree.Authentication.AdminSdk.Identity.Clients;

public interface IAccessDefinition
{
    string TypeName { get; }
}

public class ClientCredentialsAccessDefinition : IAccessDefinition
{
    public string TypeName => "client_credentials";
    public Guid[]? Roles { get; set; }

}

public class PasswordAccessDefinition : IAccessDefinition
{
    public string TypeName => "password";
}