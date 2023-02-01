using Qweree.Authentication.WebApi.Domain.Identity;
using SdkUserProperty = Qweree.Authentication.Sdk.Users.UserProperty;

namespace Qweree.Authentication.WebApi.Infrastructure.Identity;

public static class UserPropertyMapper
{
    public static SdkUserProperty Map(UserProperty property)
    {
        return new SdkUserProperty
        {
            Key = property.Key,
            Value = property.Value
        };
    }
}