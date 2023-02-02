using System.Collections.Generic;

namespace Qweree.Authentication.WebApi.Web.System;

public class HealthReport
{
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public string? Status { get; set; }

    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public Dictionary<string, HealthReportEntry>? Entries { get; set; }
}

public class HealthReportEntry
{
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public string? Status { get; set; }
}