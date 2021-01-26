using System.Linq;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Qweree.Qwill.WebApi.Web.System
{
    public static class HealthReportMapper
    {
        public static HealthReportDto ToDto(HealthReport report)
        {
            return new()
            {
                Status = report.Status.ToString(),
                Entries = report.Entries.ToDictionary(kv => kv.Key, kv => ToDto(kv.Value))
            };
        }

        public static HealthReportEntryDto ToDto(HealthReportEntry entry)
        {
            return new()
            {
                Status = entry.Status.ToString()
            };
        }
    }
}