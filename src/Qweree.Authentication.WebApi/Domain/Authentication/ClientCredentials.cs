namespace Qweree.Authentication.WebApi.Domain.Authentication;

public class ClientCredentials
{
    public ClientCredentials(string clientId, string? clientSecret)
    {
        ClientId = clientId;
        ClientSecret = clientSecret;
    }

    public string ClientId { get; }
    public string? ClientSecret { get; }
}