using Qweree.Authentication.AdminSdk.Authorization.Roles;

namespace Qweree.Authentication.AdminSdk.Identity.Clients;

public interface IAccessDefinition
{
    string TypeName { get; }
}

public class ClientCredentialsAccessDefinition : IAccessDefinition
{
    public string TypeName => "client_credentials";
    public Role[]? Roles { get; set; }

}

public class ImplicitAccessDefinition : IAccessDefinition
{
    public string TypeName => "implicit";
    public string? RedirectUri { get; set; }
}

public class AuthorizationCodeAccessDefinition : IAccessDefinition
{
    public string TypeName => "authorization_code";
    public string? RedirectUri { get; set; }
}

public class PasswordAccessDefinition : IAccessDefinition
{
    public string TypeName => "password";
}