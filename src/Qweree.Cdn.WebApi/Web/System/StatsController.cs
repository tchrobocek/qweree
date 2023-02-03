using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Qweree.Cdn.Sdk.System;
using Qweree.Cdn.WebApi.Infrastructure.System;

namespace Qweree.Cdn.WebApi.Web.System;

[ApiController]
[Route("/api/system/")]
public class StatsController : ControllerBase
{
    private readonly StatsService _statsService;

    public StatsController(StatsService statsService)
    {
        _statsService = statsService;
    }

    /// <summary>
    ///     Get cdn stats.
    /// </summary>
    /// <returns>Returns cdn stats.</returns>
    [HttpGet("stats")]
    [ProducesResponseType(typeof(CdnStatsDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStatsActionAsync()
    {
        var stats = await _statsService.ComputeCdnStatsAsync();
        var dto = new CdnStatsDto
        {
            ItemsCount = stats.MediaTypes.Sum(m => m.Count),
            DiskSpaceTotal = stats.DiskSpaceTotal,
            DiskSpaceAvailable = stats.DiskSpaceAvailable,
            SpaceUsed = stats.MediaTypes.Sum(m => m.UsedSpace),
            MediaTypes = stats.MediaTypes.Select(s => new MediaTypeStatsDto
            {
                Count = s.Count,
                MediaType = s.MediaType,
                UsedSpace = s.UsedSpace
            }).ToArray()
        };

        return Ok(dto);
    }
}