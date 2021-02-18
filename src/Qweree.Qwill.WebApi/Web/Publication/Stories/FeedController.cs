using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Qweree.AspNet.Application;
using Qweree.AspNet.Web;
using Qweree.Qwill.WebApi.Domain.Publishers;
using Qweree.Qwill.WebApi.Domain.Stories;
using Qweree.Sdk;

namespace Qweree.Qwill.WebApi.Web.Publication.Stories
{
    [ApiController]
    [Route("/api/v1/feed")]
    public class FeedController : ControllerBase
    {
        private readonly FeedService _feedService;
        private readonly ChannelService _channelService;

        public FeedController(FeedService feedService, ChannelService channelService)
        {
            _feedService = feedService;
            _channelService = channelService;
        }

        /// <summary>
        ///     Load home feed.
        /// </summary>
        /// <param name="skip">How many items should lookup to database skip. Default: 0</param>
        /// <param name="take">How many items should be returned. Default: 100</param>
        /// <returns>Returns story feed.</returns>
        [HttpGet]
        [Route("home")]
        [ProducesResponseType(typeof(List<StoryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> HomeFeedAction(
            [FromQuery(Name = "skip")] int skip = 0,
            [FromQuery(Name = "take")] int take = 10
        )
        {
            var stories = await _feedService.GetHomeFeedAsync(skip, take);
            return Ok(stories.Payload?.Select(StoryMapper.ToDto) ?? new List<StoryDto>());
        }

        /// <summary>
        ///     Load channel feed.
        /// </summary>
        /// <param name="id">Channel id.</param>
        /// <param name="skip">How many items should lookup to database skip. Default: 0</param>
        /// <param name="take">How many items should be returned. Default: 100</param>
        /// <returns>Returns story feed.</returns>
        [HttpGet]
        [Route("channel/{id}")]
        [ProducesResponseType(typeof(ChannelFeedDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ChannelFeedAction(
            Guid id,
            [FromQuery(Name = "skip")] int skip = 0,
            [FromQuery(Name = "take")] int take = 10
        )
        {
            var channelResponse = await _channelService.GetAsync(id);

            if (channelResponse.Status != ResponseStatus.Ok)
                return channelResponse.ToErrorActionResult();

            var storiesResponse = await _feedService.GetChannelFeedAsync(id, skip, take);

            if (storiesResponse.Status != ResponseStatus.Ok)
                return storiesResponse.ToErrorActionResult();

            var channel = channelResponse.Payload ?? throw new ArgumentNullException();
            var stories = storiesResponse.Payload ?? throw new ArgumentNullException();

            return Ok(new ChannelFeedDto
            {
                Id = channel.Id,
                ChannelName = channel.ChannelName,
                Stories = stories.Select(StoryMapper.ToDto).ToList()
            });
        }
    }
}