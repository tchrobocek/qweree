using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Qweree.AspNet.Session;
using Qweree.AspNet.Web;
using Qweree.AspNet.Web.Swagger;
using Qweree.Cdn.Sdk;
using Qweree.Cdn.Sdk.Storage;
using Qweree.PiccStash.WebApi.Domain;
using Qweree.Sdk;
using Qweree.Utils;

namespace Qweree.PiccStash.WebApi.Web.Piccs
{
    [ApiController]
    [Route("/api/v1/picc")]
    public class PiccController : ControllerBase
    {
        private readonly ISessionStorage _sessionStorage;
        private readonly StorageClient _storageClient;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly StashedPiccRepository _piccRepository;

        public PiccController(ISessionStorage sessionStorage, StorageClient storageClient, IDateTimeProvider dateTimeProvider, StashedPiccRepository piccRepository)
        {
            _sessionStorage = sessionStorage;
            _storageClient = storageClient;
            _dateTimeProvider = dateTimeProvider;
            _piccRepository = piccRepository;
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
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PiccCreateActionAsync([FromHeader(Name = "Content-Type")] string contentType)
        {
            if (!contentType.StartsWith("image/"))
            {
                return BadRequest("Can upload images only.");
            }

            var userId = _sessionStorage.CurrentUser.Id;
            var piccId = Guid.NewGuid();

            var slug = new[] {"apps", "picc", "usr", userId.ToString(), "stash", piccId.ToString()};
            var response = await _storageClient.StoreAsync(SlugHelper.SlugToPath(slug), contentType, Request.Body);

            if (!response.IsSuccessful)
            {
                return StatusCode((int)response.StatusCode, await response.ReadErrorsAsync());
            }

            var picc = new StashedPicc
            {
                Id = piccId,
                Name = string.Empty,
                CreatedAt = _dateTimeProvider.UtcNow,
                ModifiedAt = _dateTimeProvider.UtcNow,
                OwnerId = userId,
                StorageSlug = slug
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
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PiccsPaginateActionAsync(
            [FromQuery(Name = "sort")] Dictionary<string, string[]> sort,
            [FromQuery(Name = "skip")] int skip = 0,
            [FromQuery(Name = "take")] int take = 50
        )
        {
            var sortDictionary = sort.ToDictionary(kv => kv.Key, kv => int.Parse(kv.Value.FirstOrDefault() ?? "1"));

            var userId = _sessionStorage.CurrentUser.Id;
            var pagination = await _piccRepository.PaginateAsync($@"{{""OwnerId"": UUID(""{userId}"")}}", skip, take, sortDictionary);

            var sortParts = sort.Select(s => $"sort[{s.Key}]={s.Value}");
            Response.Headers.AddLinkHeaders($"?{string.Join("&", sortParts)}", skip, take, pagination.TotalCount);

            var dtos = pagination.Documents.Select(PiccDto);
            return Ok(dtos);
        }

        private PiccDto PiccDto(StashedPicc picc)
        {
            return new()
            {
                Id = picc.Id,
                Name = picc.Name,
                CreatedAt = picc.CreatedAt,
                ModifiedAt = picc.ModifiedAt,
                StorageSlug = picc.StorageSlug?.ToImmutableArray()
            };
        }
    }
}