namespace Qweree.Authentication.WebApi.Domain.Identity;

public class ClientSecretPair
{
    public ClientSecretPair(Client client, string secret)
    {
        Client = client;
        Secret = secret;
    }

    public Client Client { get; }
    public string Secret { get; }
}