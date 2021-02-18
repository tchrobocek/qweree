using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Qweree.AspNet.Application;
using Qweree.AspNet.Web;
using Qweree.Qwill.WebApi.Domain.Publishers;
using Qweree.Sdk;

namespace Qweree.Qwill.WebApi.Web.Publication.Publishers
{
    [ApiController]
    [Route("/api/v1/publication/channels")]
    public class ChannelController : ControllerBase
    {
        private readonly ChannelService _channelService;

        public ChannelController(ChannelService channelService)
        {
            _channelService = channelService;
        }

        /// <summary>
        ///    Returns channels collection where current user is author.
        /// </summary>
        /// <returns>Channel collection.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<ChannelDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetOwnChannelsActionAsyncAsync()
        {
            var response = await _channelService.GetOwnChannelsAsync();

            if (response.Status != ResponseStatus.Ok)
                return BadRequest(response.ToErrorResponseDto());

            return Ok(response.Payload?.Select(ChannelMapper.ToDto));
        }
    }
}