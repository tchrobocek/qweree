using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Qweree.Authentication.WebApi.Web.System;

[ApiController]
[Route("/api/system")]
public class SystemController : ControllerBase
{
    private readonly HealthCheckService _healthCheckService;

    public SystemController(HealthCheckService healthCheckService)
    {
        _healthCheckService = healthCheckService;
    }

    /// <summary>
    ///     Get Version.
    /// </summary>
    /// <returns>Returns current project assembly version.</returns>
    [HttpGet("version")]
    [ProducesResponseType(typeof(VersionDto), StatusCodes.Status200OK)]
    public IActionResult VersionGetAction()
    {
        var version = GetType().Assembly.GetName().Version?.ToString();
        return Ok(new VersionDto {Version = version});
    }

    /// <summary>
    ///     Get Version.
    /// </summary>
    /// <returns>Returns current project assembly version.</returns>
    [HttpGet("headers")]
    [ProducesResponseType(typeof(VersionDto), StatusCodes.Status200OK)]
    public IActionResult Headers()
    {
        return Ok(Request.Headers.Select(kv => new{ kv.Key, kv.Value }));
    }

    /// <summary>
    ///     Get Health.
    /// </summary>
    /// <returns>Returns health of the application.</returns>
    [HttpGet("health")]
    [ProducesResponseType(typeof(HealthReport), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(HealthReport), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> HealthGetActionAsync(CancellationToken cancellationToken = new())
    {
        var report = await _healthCheckService.CheckHealthAsync(cancellationToken);
        var reportDto = HealthReportMapper.ToHealthReport(report);

        if (report.Status == HealthStatus.Healthy) return Ok(reportDto);

        return StatusCode(StatusCodes.Status503ServiceUnavailable, reportDto);
    }
}