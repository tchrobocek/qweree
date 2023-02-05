using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Qweree.Authentication.WebApi.Infrastructure.Identity;

[BsonKnownTypes(typeof(ClientCredentialsAccessDefinitionDo), typeof(PasswordAccessDefinitionDo))]
public abstract class AccessDefinitionDo
{
}

[BsonDiscriminator("client_credentials")]
public class ClientCredentialsAccessDefinitionDo : AccessDefinitionDo
{
    public Guid[]? Roles { get; set; }

}

[BsonDiscriminator("password")]
public class PasswordAccessDefinitionDo : AccessDefinitionDo
{
}
