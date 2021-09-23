namespace Qweree.WebApplication.Infrastructure.ServicesOverview
{
    public class ServiceInfo
    {
        public string Label { get; set; } = string.Empty;
        public string Uri { get; set; } = string.Empty;
        public string SwaggerUri { get; set; } = string.Empty;
        public string Version { get; set; } = "0.0.0";
        public HealthReportDto HealthReport { get; set; } = new();
    }
}