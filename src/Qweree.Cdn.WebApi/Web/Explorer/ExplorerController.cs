using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Qweree.AspNet.Application;
using Qweree.AspNet.Web;
using Qweree.Cdn.WebApi.Application.Explorer;

namespace Qweree.Cdn.WebApi.Web.Explorer
{
    [ApiController]
    [Route("/api/v1/explorer")]
    public class ExplorerController : ControllerBase
    {
        private readonly ExplorerService _explorerService;

        public ExplorerController(ExplorerService explorerService)
        {
            _explorerService = explorerService;
        }

        /// <summary>
        ///     Get explorer object.
        /// </summary>
        /// <param name="path">Object path.</param>
        /// <returns></returns>
        [HttpGet]
        [HttpGet("{*path}")]
        [ProducesResponseType(typeof(IExplorerObjectDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExplorerDirectoryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExplorerFileDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> ExplorePathActionAsync(string? path)
        {
            path = HttpUtility.UrlDecode(path ?? "");
            var input = new ExplorerFilter(path);
            var response = await _explorerService.ExplorePathAsync(input);

            if (response.Status == ResponseStatus.Fail)
                return BadRequest(response.ToErrorResponseDto());

            return Ok(response.Payload?.Select(ExplorerObjectMapper.ToDto));
        }
    }
}