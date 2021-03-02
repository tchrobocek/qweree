using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Qweree.AspNet.Application;
using Qweree.AspNet.Web;
using Qweree.Qwill.WebApi.Domain.Stories;
using Qweree.Qwill.WebApi.Infrastructure.Publication.Stories;
using Qweree.Sdk;

namespace Qweree.Qwill.WebApi.Web.Publication.Stories
{
    [ApiController]
    [Route("/api/v1/publication/stories")]
    public class StoryController : ControllerBase
    {
        private readonly PublicationService _publicationService;

        public StoryController(PublicationService publicationService)
        {
            _publicationService = publicationService;
        }

        /// <summary>
        ///     Create story.
        /// </summary>
        /// <param name="input">Story input.</param>
        /// <returns>Returns created story.</returns>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(StoryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> StoryCreateActionAsync(PlainArticleInputDto input)
        {
            var publicationInput = PublicationInputMapper.FromDto(input);
            var response = await _publicationService.CreatePublicationAsync(publicationInput);
            if (response.Status != ResponseStatus.Ok) return BadRequest(response.ToErrorResponseDto());

            var storyDto = StoryMapper.ToDto(response.Payload ?? throw new InvalidOperationException("Empty payload."));
            return Created($"/api/v1/publication/stories/{storyDto.Id}", storyDto);
        }

        /// <summary>
        ///     Get story.
        /// </summary>
        /// <param name="id">Id.</param>
        /// <returns>Returns story.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(StoryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> StoryGetActionAsync(Guid id)
        {
            var response = await _publicationService.GetStoryAsync(id);
            if (response.Status != ResponseStatus.Ok) return BadRequest(response.ToErrorResponseDto());

            var storyDto = StoryMapper.ToDto(response.Payload ?? throw new InvalidOperationException("Empty payload."));
            return Created($"/api/v1/publication/stories/{storyDto.Id}", storyDto);
        }

        /// <summary>
        ///     Add commentary.
        /// </summary>
        /// <param name="id">Id.</param>
        /// <param name="input">Comment input</param>
        [HttpPost("{id}/commentary")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> StoryPostCommentActionAsync(Guid id, CommentInputDto input)
        {
            var response = await _publicationService.AddCommentAsync(id, new CommentInput(input.Text ?? string.Empty));

            if (response.Status != ResponseStatus.Ok)
                return BadRequest(response.ToErrorResponseDto());

            return NoContent();
        }
    }
}