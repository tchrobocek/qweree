using Microsoft.IdentityModel.Tokens;

namespace Qweree.Authentication.Sdk.Session.Tokens;

public interface ITokenEncoder
{
    public string EncodeAccessToken(AccessToken token, RsaSecurityKey rsa);
    public AccessToken DecodeAccessToken(string token);
}