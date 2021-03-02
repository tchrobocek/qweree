using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Qweree.AspNet.Application;
using Qweree.AspNet.Web;
using Qweree.Qwill.WebApi.Domain.Commentary;
using Qweree.Qwill.WebApi.Domain.Stories;
using Qweree.Qwill.WebApi.Infrastructure.Publication.Stories;
using Qweree.Qwill.WebApi.Web.Publication.Commentary;
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
        [Authorize]
        [HttpPost("{id}/commentary")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> StoryPostCommentActionAsync(Guid id, CommentInputDto input)
        {
            var response = await _publicationService.AddCommentAsync(id, new CommentInput(input.Text ?? string.Empty));

            if (response.Status != ResponseStatus.Ok)
                response.ToErrorActionResult();

            return NoContent();
        }

        /// <summary>
        ///     Paginate commentary.
        /// </summary>
        /// <param name="id">Id.</param>
        /// <param name="skip">How many items should lookup to database skip. Default: 0</param>
        /// <param name="take">How many items should be returned. Default: 10</param>
        [HttpGet("{id}/commentary")]
        [ProducesResponseType(typeof(CommentSubjectType[]), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> StoryGetCommentsActionAsync(Guid id,
            [FromQuery(Name = "skip")] int skip = 0,
            [FromQuery(Name = "take")] int take = 10)
        {
            var response = await _publicationService.PaginateCommentsAsync(id, skip, take);

            if (response.Status != ResponseStatus.Ok)
                return response.ToErrorActionResult();

            return Ok(response.Payload?.Select(SubjectCommentMapper.ToDto) ?? throw new ArgumentNullException());
        }
    }
}