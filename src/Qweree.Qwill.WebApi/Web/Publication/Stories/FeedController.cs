using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Qweree.Qwill.WebApi.Infrastructure.Publication.Stories;
using Qweree.Sdk;

namespace Qweree.Qwill.WebApi.Web.Publication.Stories
{
    [ApiController]
    [Route("/api/v1/feed")]
    public class FeedController : ControllerBase
    {
        /// <summary>
        /// Load home feed.
        /// </summary>
        /// <param name="skip">How many items should lookup to database skip. Default: 0</param>
        /// <param name="take">How many items should be returned. Default: 100</param>
        /// <returns>Returns story feed.</returns>
        [HttpGet]
        [Route("home")]
        [ProducesResponseType(typeof(StoryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        public IActionResult HomeFeedAction(
            [FromQuery(Name = "skip")] int skip = 0,
            [FromQuery(Name = "take")] int take = 10
            )
        {
            var stories = new List<StoryDto>();
            for (var i = 0; i < take; i++)
            {
                stories.Add(StoryMockFactory.CreateStory());
            }

            return Ok(stories);
        }


    }
}