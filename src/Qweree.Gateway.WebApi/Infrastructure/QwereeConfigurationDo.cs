namespace Qweree.Gateway.WebApi.Infrastructure;

public class QwereeConfigurationDo
{
    public string? AuthUri { get; set; }
    public string? SessionStorage { get; set; }
    public string? Origin { get; set; }
    public string? CdnUri { get; set; }
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
}