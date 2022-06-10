using System.Collections.Immutable;
using System.Linq;

namespace Qweree.Authentication.WebApi.Domain.Authentication;

public struct GrantType
{
    public const string PasswordRoleKey = "qweree.auth.grant_type.password";
    public const string RefreshTokenRoleKey = "qweree.auth.grant_type.refresh_token";
    public const string ClientCredentialsRoleKey = "qweree.auth.grant_type.client_credentials";

    public static readonly GrantType Password = new("password", PasswordRoleKey);
    public static readonly GrantType RefreshToken = new("refresh_token", RefreshTokenRoleKey);
    public static readonly GrantType ClientCredentials = new("client_credentials", ClientCredentialsRoleKey);

    public static readonly ImmutableArray<GrantType> GrantTypes = new[] { Password, RefreshToken, ClientCredentials }.ToImmutableArray();

    public static GrantType FromKey(string key)
    {
        return GrantTypes.Single(g => g.Key == key);
    }

    public GrantType(string key, string roleKey)
    {
        Key = key;
        RoleKey = roleKey;
    }

    public string Key { get; }
    public string RoleKey { get; }
}