namespace Qweree.Authentication.WebApi.Domain.Authentication;

public class TokenInfo
{
    public TokenInfo(string accessToken, int expiresIn) : this(accessToken, null, expiresIn)
    {
    }

    public TokenInfo(string accessToken, string? refreshToken, int expiresIn)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        ExpiresIn = expiresIn;
    }

    public string AccessToken { get; }
    public string? RefreshToken { get; }
    public int ExpiresIn { get; }
}