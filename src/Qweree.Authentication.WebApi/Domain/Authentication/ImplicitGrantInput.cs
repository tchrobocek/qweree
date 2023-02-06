namespace Qweree.Authentication.WebApi.Domain.Authentication;

public class ImplicitGrantInput
{
    public ImplicitGrantInput(string clientId, string redirectUri)
    {
        ClientId = clientId;
        RedirectUri = redirectUri;
    }

    public string ClientId { get; }
    public string RedirectUri { get; }
}