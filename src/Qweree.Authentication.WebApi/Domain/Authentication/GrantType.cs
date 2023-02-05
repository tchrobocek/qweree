namespace Qweree.Authentication.WebApi.Domain.Authentication;

public struct GrantType
{
    public static readonly GrantType Password = new("password");
    public static readonly GrantType RefreshToken = new("refresh_token");
    public static readonly GrantType ClientCredentials = new("client_credentials");

    public GrantType(string key)
    {
        Key = key;
    }

    public string Key { get; }
}