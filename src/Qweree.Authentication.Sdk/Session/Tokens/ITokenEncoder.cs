namespace Qweree.Authentication.Sdk.Session.Tokens;

public interface ITokenEncoder
{
    public string EncodeAccessToken(AccessToken token);
    public AccessToken DecodeAccessToken(string token);
}