namespace Qweree.Authentication.WebApi.Domain.Identity
{
    public class ClientCreateInput
    {
        public string ClientId { get; }
        public string ClientSecret { get; }
        public string ApplicationName { get; }
    }
}