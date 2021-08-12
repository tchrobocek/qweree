using System.Collections.Generic;

namespace Qweree.WebApplication.Infrastructure.ServicesOverview
{
    public class HealthReportDto
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string? Status { get; set; }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public Dictionary<string, HealthReportEntryDto>? Entries { get; set; }
    }

    public class HealthReportEntryDto
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string? Status { get; set; }
    }
}