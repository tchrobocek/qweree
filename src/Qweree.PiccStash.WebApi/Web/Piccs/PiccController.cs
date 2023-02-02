using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Qweree.AspNet.Web;
using Qweree.AspNet.Web.Swagger;
using Qweree.Authentication.Sdk.Session;
using Qweree.Cdn.Sdk;
using Qweree.Cdn.Sdk.Storage;
using Qweree.PiccStash.Sdk;
using Qweree.PiccStash.WebApi.Domain;
using Qweree.PiccStash.WebApi.Infrastructure;
using Qweree.Sdk;
using Qweree.Utils;

namespace Qweree.PiccStash.WebApi.Web.Piccs;

[ApiController]
[Route("/api/v1/picc")]
public class PiccController : ControllerBase
{
    private readonly ISessionStorage _sessionStorage;
    private readonly StorageClient _storageClient;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly StashedPiccRepository _piccRepository;
    private readonly IOptions<QwereeConfigurationDo> _qwereeConfiguration;

    public PiccController(ISessionStorage sessionStorage, StorageClient storageClient, IDateTimeProvider dateTimeProvider, StashedPiccRepository piccRepository, IOptions<QwereeConfigurationDo> qwereeConfiguration)
    {
        _sessionStorage = sessionStorage;
        _storageClient = storageClient;
        _dateTimeProvider = dateTimeProvider;
        _piccRepository = piccRepository;
        _qwereeConfiguration = qwereeConfiguration;
    }

    /// <summary>
    ///     Create picc.
    /// </summary>
    /// <param name="contentType">Content type.</param>
    /// <returns>Created picc.</returns>
    [HttpPost]
    [Authorize]
    [RequiresFileFromBody]
    [ProducesResponseType(typeof(PiccDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PiccCreateActionAsync([FromHeader(Name = "Content-Type")] string contentType)
    {
        if (!contentType.StartsWith("image/"))
        {
            return BadRequest("Can upload images only.");
        }

        var userId = _sessionStorage.UserId;
        var piccId = Guid.NewGuid();

        var path = PathHelper.Combine(PathHelper.GetClientDataPath(_qwereeConfiguration.Value.ClientId ?? string.Empty), "piccs", piccId.ToString());
        var response = await _storageClient.StoreAsync(path, contentType, Request.Body);
        response.EnsureSuccessStatusCode();

        var descriptor = await response.ReadPayloadAsync();
        var extension = contentType["image/".Length..];

        var picc = new StashedPicc
        {
            Id = piccId,
            Name = piccId + "." + extension,
            CreatedAt = _dateTimeProvider.UtcNow,
            ModifiedAt = _dateTimeProvider.UtcNow,
            OwnerId = userId,
            StorageSlug = PathHelper.PathToSlug(path),
            Size = descriptor?.Size,
            MediaType = descriptor?.MediaType
        };

        try
        {
            await _piccRepository.InsertAsync(picc);
        }
        catch (Exception)
        {
            return BadRequest("Bad request.");
        }

        return Ok(PiccDto(picc));
    }

    /// <summary>
    ///     Read picc.
    /// </summary>
    /// <param name="piccId">Picc id.</param>
    [HttpGet("{piccId:guid}")]
    [ProducesResponseType(typeof(byte[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PiccReadActionAsync(Guid piccId)
    {
        StashedPicc picc;

        try
        {
            picc = await _piccRepository.GetAsync(piccId);
        }
        catch (Exception)
        {
            return NotFound();
        }

        var ifNoneMatchValue = Request.Headers.IfNoneMatch.FirstOrDefault();

        var response = await _storageClient.RetrieveAsync(PathHelper.SlugToPath(picc.StorageSlug!), ifNoneMatchValue);
        response.EnsureSuccessStatusCode();

        var mimeType = MediaTypeNames.Application.Octet;
        if (response.ContentHeaders.TryGetValues("Content-Type", out var mimeTypes))
            mimeType = mimeTypes.Single();

        if (response.ResponseHeaders.TryGetValues("ETag", out var etags))
            Response.Headers.ETag = etags.FirstOrDefault();

        if (response.StatusCode == HttpStatusCode.NotModified)
        {
            response.Dispose();
            return StatusCode((int)HttpStatusCode.NotModified);
        }

        var stream = await response.ReadPayloadAsStreamAsync();
        return File(stream, mimeType);
    }

    /// <summary>
    ///     Download picc.
    /// </summary>
    /// <param name="piccId">Picc id.</param>
    [HttpGet("{piccId:guid}/download")]
    [Authorize]
    [RequiresFileFromBody]
    [ProducesResponseType(typeof(byte[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PiccDownloadActionAsync(Guid piccId)
    {
        StashedPicc picc;

        try
        {
            picc = await _piccRepository.GetAsync(piccId);
        }
        catch (Exception)
        {
            return NotFound();
        }

        var response = await _storageClient.RetrieveAsync(PathHelper.SlugToPath(picc.StorageSlug!));
        response.EnsureSuccessStatusCode();

        string mimeType = MediaTypeNames.Application.Octet;

        if (response.ContentHeaders.TryGetValues("Content-Type", out var mimeTypes))
        {
            mimeType = mimeTypes.Single();
        }


        await using var stream = await response.ReadPayloadAsStreamAsync();
        return File(stream, mimeType, picc.Name);
    }

    /// <summary>
    ///     Find piccs
    /// </summary>
    /// <param name="skip">How many items should lookup to database skip. Default: 0</param>
    /// <param name="take">How many items should be returned. Default: 100</param>
    /// <param name="sort">
    ///     Sorting.
    ///     Asc 1; Desc -1
    ///     sort[CreatedAt]=-1 // sort by created, desc.
    /// </param>
    /// <returns>Collection of piccs.</returns>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(List<PiccDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PiccsPaginateActionAsync(
        [FromQuery(Name = "sort")] Dictionary<string, string[]> sort,
        [FromQuery(Name = "skip")] int skip = 0,
        [FromQuery(Name = "take")] int take = 50
    )
    {
        var sortDictionary = sort.ToDictionary(kv => kv.Key, kv => int.Parse(kv.Value.FirstOrDefault() ?? "1"));

        var userId = _sessionStorage.UserId;
        var pagination = await _piccRepository.PaginateAsync($@"{{""OwnerId"": UUID(""{userId}"")}}", skip, take, sortDictionary);

        var sortParts = sort.Select(s => $"sort[{s.Key}]={s.Value}");
        Response.Headers.AddLinkHeaders($"?{string.Join("&", sortParts)}", skip, take, pagination.TotalCount);

        Response.Headers.Add("q-document-count", new[] { pagination.TotalCount.ToString() });

        var dtos = pagination.Documents.Select(PiccDto);
        return Ok(dtos);
    }

    /// <summary>
    ///     Delete picc.
    /// </summary>
    /// <param name="piccId">Picc to delete.</param>
    [HttpDelete("{piccId:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PiccDeleteActionAsync(Guid piccId)
    {
        StashedPicc picc;

        try
        {
            picc = await _piccRepository.GetAsync(piccId);
        }
        catch (Exception)
        {
            return NotFound();
        }

        var response = await _storageClient.DeleteAsync(PathHelper.SlugToPath(picc.StorageSlug ?? Array.Empty<string>()));
        response.EnsureSuccessStatusCode();
        await _piccRepository.DeleteOneAsync(piccId);

        return NoContent();
    }

    private PiccDto PiccDto(StashedPicc picc)
    {
        return new PiccDto
        {
            Id = picc.Id,
            Name = picc.Name,
            Size = picc.Size,
            MediaType = picc.MediaType,
            CreatedAt = picc.CreatedAt,
            ModifiedAt = picc.ModifiedAt
        };
    }
}