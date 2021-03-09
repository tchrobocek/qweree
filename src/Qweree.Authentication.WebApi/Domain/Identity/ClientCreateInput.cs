namespace Qweree.Authentication.WebApi.Domain.Identity
{
    public class ClientCreateInput
    {
        public ClientCreateInput(string clientId, string clientSecret, string applicationName)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
            ApplicationName = applicationName;
        }

        public string ClientId { get; }
        public string ClientSecret { get; }
        public string ApplicationName { get; }
    }
}