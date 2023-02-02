using System.Linq;

namespace Qweree.Authentication.WebApi.Web.System;

public static class HealthReportMapper
{
    public static HealthReport ToHealthReport(Microsoft.Extensions.Diagnostics.HealthChecks.HealthReport report)
    {
        return new HealthReport
        {
            Status = report.Status.ToString(),
            Entries = report.Entries.ToDictionary(kv => kv.Key, kv => ToHealthReportEntry(kv.Value))
        };
    }

    public static HealthReportEntry ToHealthReportEntry(Microsoft.Extensions.Diagnostics.HealthChecks.HealthReportEntry entry)
    {
        return new HealthReportEntry
        {
            Status = entry.Status.ToString()
        };
    }
}