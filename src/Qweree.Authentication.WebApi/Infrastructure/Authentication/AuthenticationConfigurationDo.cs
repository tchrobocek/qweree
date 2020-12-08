namespace Qweree.Authentication.WebApi.Infrastructure.Authentication
{
    public class AuthenticationConfigurationDo
    {
        public string? AccessTokenKey { get; set; }
        public string? FileAccessTokenKey { get; set; }
        public int? AccessTokenValiditySeconds { get; set; }
        public int? RefreshTokenValiditySeconds { get; set; }
        public int? FileAccessTokenValiditySeconds { get; set; }
    }
}