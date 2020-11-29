using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Qweree.AspNet.Application;
using Qweree.AspNet.Web;
using Qweree.AspNet.Web.Swagger;
using Qweree.Cdn.WebApi.Application.Storage;

namespace Qweree.Cdn.WebApi.Web.Storage
{
    [ApiController]
    [Route("/api/v1/storage")]
    public class StorageController : ControllerBase
    {
        private readonly StoredObjectService _service;

        public StorageController(StoredObjectService service)
        {
            _service = service;
        }

        /// <summary>
        /// Get object.
        /// </summary>
        /// <param name="slug">Object slug.</param>
        /// <returns></returns>
        [HttpGet("{*slug}")]
        [Authorize]
        [ProducesResponseType(typeof(byte[]), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetStoredObjectActionAsync(string slug)
        {
            slug = HttpUtility.UrlDecode(slug);
            var input = new ReadObjectInput(slug);
            var response = await _service.ReadObjectAsync(input);

            if (response.Status == ResponseStatus.Fail)
                return BadRequest(response.ToErrorResponseDto());

            return File(response.Payload?.Stream, response.Payload?.Descriptor.MediaType);
        }


        /// <summary>
        /// Store object.
        /// </summary>
        /// <param name="slug">Object slug.</param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        [HttpPost("{*slug}")]
        [Authorize]
        [RequiresFileFromBody]
        [ProducesResponseType(typeof(StoredObjectDescriptorDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> PostStoredObjectActionAsync(string slug, [FromHeader(Name = "Content-Type")] string contentType)
        {
            slug = HttpUtility.UrlDecode(slug);
            var input = new StoreObjectInput(slug, contentType, Request.ContentLength ?? 0, Request.Body);
            var response = await _service.StoreObjectAsync(input);

            if (response.Status == ResponseStatus.Fail)
                return BadRequest(response.ToErrorResponseDto());

            return Created($"/api/v1/storage/{slug.Trim('/')}", StoredObjectDescriptorMapper.ToDto(response.Payload?.Descriptor!));
        }
    }
}