using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
        ///    Returns channel
        /// </summary>
        /// <returns>Channel collection.</returns>
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(ChannelDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetChannelAsync(Guid id)
        {
            var response = await _channelService.GetChannelAsync(id);

            if (response.Status != ResponseStatus.Ok)
                return response.ToErrorActionResult();

            return Ok(ChannelMapper.ToDto(response.Payload!));
        }

        /// <summary>
        ///    Returns channels collection where current user is author.
        /// </summary>
        /// <returns>Channel collection.</returns>
        [Authorize]
        [HttpGet]
        [Route("own")]
        [ProducesResponseType(typeof(List<ChannelDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetOwnChannelsActionAsyncAsync()
        {
            var response = await _channelService.GetOwnChannelsAsync();

            if (response.Status != ResponseStatus.Ok)
                return BadRequest(response.ToErrorResponseDto());

            return Ok(response.Payload?.Select(ChannelMapper.ToDto));
        }

        /// <summary>
        ///    Creates channel and sets current user as owner.
        /// </summary>
        /// <returns>Created channel.</returns>
        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(ChannelDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ChannelCreateActionAsync(ChannelInputDto inputDto)
        {
            var input = new ChannelCreateInput(inputDto.ChannelName ?? string.Empty);
            var response = await _channelService.CreateAsync(input);

            if (response.Status != ResponseStatus.Ok)
                return BadRequest(response.ToErrorResponseDto());

            return Created($"/api/v1/publication/channels/{response.Payload?.Id}", ChannelMapper.ToDto(response.Payload ?? throw new ArgumentNullException(nameof(response.Payload))));
        }
    }
}