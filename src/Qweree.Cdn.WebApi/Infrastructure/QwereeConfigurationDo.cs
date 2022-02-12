namespace Qweree.Cdn.WebApi.Infrastructure;

public class QwereeConfigurationDo
{
    public string? MongoConnectionString { get; set; }
    public string? HealthCheckConnectionString { get; set; }
    public string? DatabaseName { get; set; }
    public string? AccessTokenKey { get; set; }
    public string? PathBase { get; set; }
    public string? FileSystemRoot { get; set; }
    public string? FileSystemTemp { get; set; }
    public string? SwaggerTokenUri { get; set; }
    public string? AuthTokenUri { get; set; }
}