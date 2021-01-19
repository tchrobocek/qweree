using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Qweree.Qwill.WebApi.Infrastructure.Publication.Stories;
using Qweree.Sdk;

namespace Qweree.Qwill.WebApi.Web.Publication.Stories
{
    [ApiController]
    [Route("/api/v1/publication/stories")]
    public class StoryController : ControllerBase
    {
        /// <summary>
        /// Create story.
        /// </summary>
        /// <param name="input">Story input.</param>
        /// <returns>Returns created story.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(StoryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        public IActionResult StoryCreateActionAsync(PlainArticleInputDto input)
        {
            return Ok(StoryMockFactory.CreateStory());
        }

        /// <summary>
        /// Get story.
        /// </summary>
        /// <param name="id">Id.</param>
        /// <returns>Returns story.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(StoryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        public IActionResult GetArticleActionAsync(Guid id)
        {
            return Ok(StoryMockFactory.CreateStory());
        }
    }
}