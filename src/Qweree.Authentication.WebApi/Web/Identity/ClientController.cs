using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Qweree.AspNet.Application;
using Qweree.AspNet.Web;
using Qweree.Authentication.Sdk.Identity;
using Qweree.Authentication.WebApi.Domain.Identity;
using Qweree.Sdk;

namespace Qweree.Authentication.WebApi.Web.Identity
{
    [ApiController]
    [Route("/api/v1/identity/clients")]
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

            var createdClient = ClientToCreatedClient(clientResponse.Payload!);
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

            var createdClient = ClientToDto(clientResponse.Payload!);
            return Ok(createdClient);
        }

        private CreatedClientDto ClientToCreatedClient(Client client)
        {
            return new()
            {
                Id = client.Id,
                ApplicationName = client.ApplicationName,
                ClientId = client.ClientId,
                ClientSecret = client.ClientSecret,
                CreatedAt = client.CreatedAt,
                ModifiedAt = client.ModifiedAt,
                OwnerId = client.OwnerId
            };
        }

        private ClientDto ClientToDto(Client client)
        {
            return new()
            {
                Id = client.Id,
                ApplicationName = client.ApplicationName,
                ClientId = client.ClientId,
                CreatedAt = client.CreatedAt,
                ModifiedAt = client.ModifiedAt,
                OwnerId = client.OwnerId
            };
        }
    }
}