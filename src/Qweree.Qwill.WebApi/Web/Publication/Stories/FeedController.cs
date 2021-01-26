using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Qweree.Qwill.WebApi.Domain.Stories;
using Qweree.Sdk;

namespace Qweree.Qwill.WebApi.Web.Publication.Stories
{
    [ApiController]
    [Route("/api/v1/feed")]
    public class FeedController : ControllerBase
    {
        private readonly FeedService _feedService;

        public FeedController(FeedService feedService)
        {
            _feedService = feedService;
        }

        /// <summary>
        ///     Load home feed.
        /// </summary>
        /// <param name="skip">How many items should lookup to database skip. Default: 0</param>
        /// <param name="take">How many items should be returned. Default: 100</param>
        /// <returns>Returns story feed.</returns>
        [HttpGet]
        [Route("home")]
        [ProducesResponseType(typeof(StoryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> HomeFeedAction(
            [FromQuery(Name = "skip")] int skip = 0,
            [FromQuery(Name = "take")] int take = 10
        )
        {
            var stories = await _feedService.GetHomeFeedAsync(skip, take);
            return Ok(stories.Payload?.Select(StoryMapper.ToDto) ?? new List<StoryDto>());
        }
    }
}