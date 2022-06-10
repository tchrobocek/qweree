namespace Qweree.PiccStash.WebApi.Infrastructure;

public class QwereeConfigurationDo
{
    public string? MongoConnectionString { get; set; }
    public string? HealthCheckConnectionString { get; set; }
    public string? DatabaseName { get; set; }
    public string? AccessTokenKey { get; set; }
    public string? PathBase { get; set; }
    public string? SwaggerTokenUri { get; set; }
    public string? AuthUri { get; set; }
    public string? CdnUri { get; set; }
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
}