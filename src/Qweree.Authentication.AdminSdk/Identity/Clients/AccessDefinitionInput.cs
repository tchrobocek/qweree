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