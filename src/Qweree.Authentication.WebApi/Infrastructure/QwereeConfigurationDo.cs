namespace Qweree.Authentication.WebApi.Infrastructure;

public class QwereeConfigurationDo
{
    public string? MongoConnectionString { get; set; }
    public string? HealthCheckConnectionString { get; set; }
    public string? DatabaseName { get; set; }
    public string? PasswordKey { get; set; }
    public string? AccessTokenKey { get; set; }
    public string? PathBase { get; set; }
    public int? AccessTokenValiditySeconds { get; set; }
    public int? RefreshTokenValiditySeconds { get; set; }
}