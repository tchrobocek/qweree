using System.Linq;
using Qweree.Authentication.AdminSdk.Identity.Users;
using Qweree.Authentication.Sdk.Identity;
using Qweree.Authentication.Sdk.Session;

namespace Qweree.WebApplication.Infrastructure.Extensions;

public static class IdentityExtensions
{
    public static string? GetFullName(this User @this)
    {
        var fullNameProperty = @this.Properties?.FirstOrDefault(p => p.Key == UserProperties.FullName);
        return fullNameProperty?.Value;
    }

    public static string? GetFullName(this IdentityUser @this)
    {
        var fullNameProperty = @this.Properties?.FirstOrDefault(p => p.Key == UserProperties.FullName);
        return fullNameProperty?.Value;
    }
}