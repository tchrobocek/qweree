using System.Linq;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Qweree.Cdn.WebApi.Web.System
{
    public static class HealthReportMapper
    {
        public static HealthReportDto ToDto(HealthReport report)
        {
            return new HealthReportDto
            {
                Status = report.Status.ToString(),
                Entries = report.Entries.ToDictionary(kv => kv.Key, kv => ToDto(kv.Value))
            };
        }

        public static HealthReportEntryDto ToDto(HealthReportEntry entry)
        {
            return new HealthReportEntryDto
            {
                Status = entry.Status.ToString()
            };
        }
    }
}