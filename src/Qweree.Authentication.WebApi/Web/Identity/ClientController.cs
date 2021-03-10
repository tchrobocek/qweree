using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Qweree.AspNet.Application;
using Qweree.AspNet.Web;
using Qweree.Authentication.AdminSdk.Identity.Clients;
using Qweree.Authentication.WebApi.Domain.Identity;
using Qweree.Sdk;

namespace Qweree.Authentication.WebApi.Web.Identity
{
    [ApiController]
    [Route("/api/admin/identity/clients")]
    public class ClientController : ControllerBase
    {
        private readonly ClientService _clientService;

        public ClientController(ClientService clientService)
        {
            _clientService = clientService;
        }

        /// <summary>
        ///     Create client.
        /// </summary>
        /// <param name="input">Create clint input.</param>
        /// <returns>Created client.</returns>
        [HttpPost]
        [Authorize(Policy = "ClientCreate")]
        [ProducesResponseType(typeof(CreatedClientDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateClientActionAsync(ClientCreateInputDto input)
        {
            var serviceInput = new ClientCreateInput(input.Id ?? Guid.NewGuid(), input.ClientId ?? string.Empty, input.ClientSecret ?? string.Empty,
                input.ApplicationName ?? string.Empty, input.Origin ?? string.Empty, input.OwnerId ?? Guid.Empty);

            var clientResponse = await _clientService.CreateClientAsync(serviceInput);

            if (clientResponse.Status != ResponseStatus.Ok)
                return clientResponse.ToErrorActionResult();

            var createdClient = CreatedClientMapper.ToDto(clientResponse.Payload!);
            return Created($"/api/v1/clients/{createdClient.Id}", createdClient);
        }


        /// <summary>
        ///     Get client.
        /// </summary>
        /// <param name="id">Client id.</param>
        /// <returns>Client.</returns>
        [HttpGet]
        [Route("{id}")]
        [Authorize(Policy = "ClientRead")]
        [ProducesResponseType(typeof(ClientDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetClientActionAsync(Guid id)
        {
            var clientResponse = await _clientService.GetClientAsync(id);

            if (clientResponse.Status != ResponseStatus.Ok)
                return clientResponse.ToErrorActionResult();

            var createdClient = ClientMapper.ToDto(clientResponse.Payload!);
            return Ok(createdClient);
        }

        /// <summary>
        ///     Delete client.
        /// </summary>
        /// <param name="id">Client id.</param>
        [HttpDelete]
        [Route("{id}")]
        [Authorize(Policy = "ClientDelete")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteClientActionAsync(Guid id)
        {
            var clientResponse = await _clientService.DeleteClientAsync(id);

            if (clientResponse.Status != ResponseStatus.Ok)
                return clientResponse.ToErrorActionResult();

            return NoContent();
        }

        /// <summary>
        ///     Find clients
        /// </summary>
        /// <param name="skip">How many items should lookup to database skip. Default: 0</param>
        /// <param name="take">How many items should be returned. Default: 100</param>
        /// <param name="sort">
        ///     Sorting.
        ///     Asc 1; Desc -1
        ///     sort[CreatedAt]=-1 // sort by created, desc.
        /// </param>
        /// <returns>Collection of clients.</returns>
        [HttpGet]
        [Authorize(Policy = "ClientRead")]
        [ProducesResponseType(typeof(List<ClientDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> FindClientsActionAsync(
            [FromQuery(Name = "sort")] Dictionary<string, string[]> sort,
            [FromQuery(Name = "skip")] int skip = 0,
            [FromQuery(Name = "take")] int take = 50
        )
        {
            var sortDictionary = sort.ToDictionary(kv => kv.Key, kv => int.Parse(kv.Value.FirstOrDefault() ?? "1"));
            var clientsResponse = await _clientService.PaginateClientsAsync(skip, take, sortDictionary);

            if (clientsResponse.Status != ResponseStatus.Ok)
                return clientsResponse.ToErrorActionResult();

            var sortParts = sort.Select(s => $"sort[{s.Key}]={s.Value}");
            Response.Headers.AddLinkHeaders($"?{string.Join("&", sortParts)}", skip, take, clientsResponse.DocumentCount);

            var usersDto = clientsResponse.Payload?.Select(ClientMapper.ToDto);
            return Ok(usersDto);
        }
    }
}