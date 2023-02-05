namespace Qweree.Cdn.WebApi.Infrastructure;

public class QwereeConfigurationDo
{
    public string? MongoConnectionString { get; set; }
    public string? HealthCheckConnectionString { get; set; }
    public string? DatabaseName { get; set; }
    public string? PathBase { get; set; }
    public string? FileSystemRoot { get; set; }
    public string? FileSystemTemp { get; set; }
    public string? SwaggerOpenId { get; set; }
    public string? AuthUri { get; set; }
}