using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Qweree.Qwill.WebApi.Domain.Stories;
using Qweree.Qwill.WebApi.Infrastructure.Publication.Stories;

namespace Qweree.Qwill.WebApi.Web.System
{
    [ApiController]
    [Route("/api/v1/_system")]
    public class SystemController : ControllerBase
    {
        private readonly HealthCheckService _healthCheckService;
        private readonly IPublicationRepository _publicationRepository;

        public SystemController(HealthCheckService healthCheckService, IPublicationRepository publicationRepository)
        {
            _healthCheckService = healthCheckService;
            _publicationRepository = publicationRepository;
        }

        /// <summary>
        ///     Get Version.
        /// </summary>
        /// <returns>Returns current project assembly version.</returns>
        [HttpGet("version")]
        [ProducesResponseType(typeof(VersionDto), StatusCodes.Status200OK)]
        public IActionResult GetVersionAction()
        {
            var version = GetType().Assembly.GetName().Version?.ToString();
            return Ok(new VersionDto {Version = version});
        }

        /// <summary>
        ///     Get Health.
        /// </summary>
        /// <returns>Returns health of the application.</returns>
        [HttpGet("health")]
        [ProducesResponseType(typeof(HealthReportDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HealthReportDto), StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> GetHealthActionAsync(CancellationToken cancellationToken = new())
        {
            var report = await _healthCheckService.CheckHealthAsync(cancellationToken);
            var reportDto = HealthReportMapper.ToDto(report);

            if (report.Status == HealthStatus.Healthy) return Ok(reportDto);

            return StatusCode(StatusCodes.Status503ServiceUnavailable, reportDto);
        }

        /// <summary>
        ///     Will generate and persist up to 100 stories.
        /// </summary>
        /// <returns>Returns health of the application.</returns>
        [HttpGet("mock-stories")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> MockStoriesAction(
                [FromQuery(Name = "number")] int number = 10
            )
        {
            if (number > 100)
            {
                number = 100;
            }

            for (var i = 0; i < number; i++)
            {
                var publication = PublicationMockFactory.CreatePublication();
                await _publicationRepository.InsertAsync(publication);
            }

            return NoContent();
        }
    }
}