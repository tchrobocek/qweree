namespace Qweree.Authentication.WebApi.Infrastructure.Authentication
{
    public class AuthenticationConfigurationDo
    {
        public string? AccessTokenKey { get; set; }
        public int? AccessTokenValiditySeconds { get; set; }
        public int? RefreshTokenValiditySeconds { get; set; }
    }
}