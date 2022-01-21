using System.Collections.Generic;

namespace Qweree.WebApplication.Infrastructure.ServicesOverview;

public class HealthReportDto
{
    public string? Status { get; set; }

    public Dictionary<string, HealthReportEntryDto>? Entries { get; set; }
}

public class HealthReportEntryDto
{
    public string? Status { get; set; }
}