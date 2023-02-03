using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Qweree.AspNet.Application;
using Qweree.AspNet.Web;
using Qweree.AspNet.Web.Swagger;
using Qweree.Cdn.Sdk.Storage;
using Qweree.Cdn.WebApi.Domain.Storage;
using Qweree.Cdn.WebApi.Infrastructure.Storage;

namespace Qweree.Cdn.WebApi.Web.Storage;

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
    ///     Get object.
    /// </summary>
    /// <param name="path">Object path.</param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpGet("{*path}")]
    [ProducesResponseType(typeof(byte[]), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStoredObjectActionAsync(string path)
    {
        path = HttpUtility.UrlDecode(path);
        var input = new ReadObjectInput(path);
        var response = await _service.ReadObjectAsync(input);

        if (response.Status == ResponseStatus.Fail)
            return response.ToErrorActionResult();

        var descriptor = response.Payload?.Descriptor!;
        Response.Headers.ETag = new StringValues(EtagHelper.ComputeEtag(descriptor));

        var ifNoneMatchValues = Request.Headers.IfNoneMatch.ToArray();
        if (ifNoneMatchValues.Any(v => EtagHelper.ValidateEtag(v ?? string.Empty, descriptor)))
        {
            var stream = response.Payload?.Stream;
            if (stream != null)
                await stream.DisposeAsync();

            return StatusCode((int)HttpStatusCode.NotModified);
        }

        return File(response.Payload!.Stream, response.Payload.Descriptor.MediaType);
    }

    /// <summary>
    ///     Store object.
    /// </summary>
    /// <param name="path">Object path.</param>
    /// <param name="contentType">Content type.</param>
    /// <param name="privateString">Private resource flag.</param>
    /// <returns></returns>
    [HttpPost("{*path}")]
    [Authorize(Policy = "StorageStore")]
    [RequiresFileFromBody]
    [RequestSizeLimit(1000 * 1000 * 1000)]
    [ProducesResponseType(typeof(StoredObjectDescriptorDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> PostStoredObjectActionAsync(string path,
        [FromHeader(Name = "X-Private")] string? privateString,
        [FromHeader(Name = "Content-Type")] string contentType = MediaTypeNames.Application.Octet)
    {
        bool? isPrivate = null;
        if (privateString != null)
            isPrivate = privateString != "false";

        path = HttpUtility.UrlDecode(path);
        var input = new StoreObjectInput(path, contentType, Request.Body, false, isPrivate);
        var response = await _service.StoreOrReplaceObjectAsync(input);

        if (response.Status == ResponseStatus.Fail)
            return response.ToErrorActionResult();

        return Created($"/api/v1/storage/{path.Trim('/')}",
            StoredObjectDescriptorMapper.ToDto(response.Payload!));
    }

    /// <summary>
    ///     Store object.
    /// </summary>
    /// <param name="path">Object path.</param>
    /// <param name="contentType">Content type.</param>
    /// <param name="privateString">Private resource flag.</param>
    /// <returns></returns>
    [HttpPut("{*path}")]
    [Authorize(Policy = "StorageStoreForce")]
    [RequiresFileFromBody]
    [RequestSizeLimit(1000 * 1000 * 1000)]
    [ProducesResponseType(typeof(StoredObjectDescriptorDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> PutStoredObjectActionAsync(string path,
        [FromHeader(Name = "X-Private")] string? privateString,
        [FromHeader(Name = "Content-Type")] string contentType = MediaTypeNames.Application.Octet)
    {
        bool? isPrivate = null;
        if (privateString != null)
            isPrivate = privateString != "false";

        path = HttpUtility.UrlDecode(path);
        var input = new StoreObjectInput(path, contentType, Request.Body, true, isPrivate);
        var response = await _service.StoreOrReplaceObjectAsync(input);

        if (response.Status == ResponseStatus.Fail)
            return response.ToErrorActionResult();

        return Created($"/api/v1/storage/{path.Trim('/')}",
            StoredObjectDescriptorMapper.ToDto(response.Payload!));
    }


    /// <summary>
    ///     Store object.
    /// </summary>
    /// <param name="path">Object path.</param>
    /// <returns></returns>
    [HttpDelete("{*path}")]
    [Authorize(Policy = "StorageDelete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteStoredObjectActionAsync(string path)
    {
        path = HttpUtility.UrlDecode(path);
        var response = await _service.DeleteObjectAsync(path);

        if (response.Status == ResponseStatus.Fail)
            return response.ToErrorActionResult();

        return NoContent();
    }
}