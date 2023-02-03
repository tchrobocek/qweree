using System.Linq;
using Qweree.Authentication.Sdk.Identity;
using Qweree.Authentication.WebApi.Domain.Identity;
using Qweree.Authentication.WebApi.Domain.Session;
using SdkIdentity = Qweree.Authentication.Sdk.Session.Identity;
using SdkIdentityClient = Qweree.Authentication.Sdk.Session.IdentityClient;
using SdkIdentityUser = Qweree.Authentication.Sdk.Session.IdentityUser;

namespace Qweree.Authentication.WebApi.Infrastructure.Session;

public static class IdentityMapper
{
    public static SdkIdentity Map(Domain.Session.Identity identity)
    {
        var sdkIdentity = new SdkIdentity
        {
            Client = Map(identity.Client),
            Email = identity.Email,
            Roles = identity.Roles.ToArray()
        };

        if (identity.User != null)
            sdkIdentity.User = Map(identity.User);

        return sdkIdentity;
    }

    public static SdkIdentityClient Map(IdentityClient identity)
    {
        return new SdkIdentityClient
        {
            Id = identity.Id,
            ApplicationName = identity.ApplicationName,
            ClientId = identity.ClientId
        };
    }

    public static SdkIdentityUser Map(IdentityUser identity)
    {
        return new SdkIdentityUser
        {
            Id = identity.Id,
            Username = identity.Username,
            Properties = identity.Properties.Select(Map).ToArray()
        };
    }
    public static AuthUserProperty Map(UserProperty property)
    {
        return new AuthUserProperty
        {
            Key = property.Key,
            Value = property.Value
        };
    }
}