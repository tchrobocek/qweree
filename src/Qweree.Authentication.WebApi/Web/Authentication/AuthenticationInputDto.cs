namespace Qweree.Authentication.WebApi.Web.Authentication
{
    public class AuthenticationInputDto
    {
        public string? GrantType { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
    }
}