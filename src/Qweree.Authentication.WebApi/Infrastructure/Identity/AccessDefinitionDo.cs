using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Qweree.Authentication.WebApi.Infrastructure.Identity;

[BsonKnownTypes(typeof(ClientCredentialsAccessDefinitionDo), typeof(PasswordAccessDefinitionDo),
    typeof(AuthorizationCodeAccessDefinitionDo), typeof(ImplicitAccessDefinitionDo))]
public abstract class AccessDefinitionDo
{
}

[BsonDiscriminator("client_credentials")]
public class ClientCredentialsAccessDefinitionDo : AccessDefinitionDo
{
    public Guid[]? Roles { get; set; }
}

[BsonDiscriminator("implicit")]
public class ImplicitAccessDefinitionDo : AccessDefinitionDo
{
    public string? RedirectUri { get; set; }
}

[BsonDiscriminator("authorization_code")]
public class AuthorizationCodeAccessDefinitionDo : AccessDefinitionDo
{
    public string? RedirectUri { get; set; }
}

[BsonDiscriminator("password")]
public class PasswordAccessDefinitionDo : AccessDefinitionDo
{
}