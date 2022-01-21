namespace Qweree.Authentication.Sdk.OAuth2;

public class RefreshTokenGrantInput
{
    public RefreshTokenGrantInput(string refreshToken)
    {
        RefreshToken = refreshToken;
    }

    public string RefreshToken { get; }
}