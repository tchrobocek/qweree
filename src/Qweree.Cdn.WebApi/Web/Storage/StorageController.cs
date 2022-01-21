using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Qweree.AspNet.Application;
using Qweree.AspNet.Web;
using Qweree.AspNet.Web.Swagger;
using Qweree.Cdn.Sdk.Storage;
using Qweree.Cdn.WebApi.Domain.Storage;

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
    [HttpGet("{*path}")]
    [ProducesResponseType(typeof(byte[]), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStoredObjectActionAsync(string path)
    {
        path = HttpUtility.UrlDecode(path);
        var input = new ReadObjectInput(path);
        var response = await _service.ReadObjectAsync(input);

        if (response.Status == ResponseStatus.Fail)
            return BadRequest(response.ToErrorResponseDto());

        return File(response.Payload!.Stream, response.Payload.Descriptor.MediaType);
    }

    /// <summary>
    ///     Store object.
    /// </summary>
    /// <param name="path">Object path.</param>
    /// <param name="contentType"></param>
    /// <returns></returns>
    [HttpPost("{*path}")]
    [Authorize(Policy = "StorageStore")]
    [RequiresFileFromBody]
    [RequestSizeLimit(1000 * 1000 * 1000)]
    [ProducesResponseType(typeof(StoredObjectDescriptorDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> PostStoredObjectActionAsync(string path,
        [FromHeader(Name = "Content-Type")] string contentType)
    {
        path = HttpUtility.UrlDecode(path);
        var input = new StoreObjectInput(path, contentType, Request.Body, Request.Method.ToLower() == "put");
        var response = await _service.StoreOrReplaceObjectAsync(input);

        if (response.Status == ResponseStatus.Fail)
            return BadRequest(response.ToErrorResponseDto());

        return Created($"/api/v1/storage/{path.Trim('/')}",
            StoredObjectDescriptorMapper.ToDto(response.Payload?.Descriptor!));
    }

    /// <summary>
    ///     Store object.
    /// </summary>
    /// <param name="path">Object path.</param>
    /// <param name="contentType"></param>
    /// <returns></returns>
    [HttpPut("{*path}")]
    [Authorize(Policy = "StorageStoreForce")]
    [RequiresFileFromBody]
    [RequestSizeLimit(1000 * 1000 * 1000)]
    [ProducesResponseType(typeof(StoredObjectDescriptorDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> PutStoredObjectActionAsync(string path,
        [FromHeader(Name = "Content-Type")] string contentType)
    {
        path = HttpUtility.UrlDecode(path);
        var input = new StoreObjectInput(path, contentType, Request.Body, Request.Method.ToLower() == "put");
        var response = await _service.StoreOrReplaceObjectAsync(input);

        if (response.Status == ResponseStatus.Fail)
            return BadRequest(response.ToErrorResponseDto());

        return Created($"/api/v1/storage/{path.Trim('/')}",
            StoredObjectDescriptorMapper.ToDto(response.Payload?.Descriptor!));
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
            return BadRequest(response.ToErrorResponseDto());

        return NoContent();
    }
}