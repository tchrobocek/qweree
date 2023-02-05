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
using Qweree.Authentication.WebApi.Domain.Session;
using Qweree.Authentication.WebApi.Infrastructure;
using Qweree.Authentication.WebApi.Infrastructure.Identity;
using Qweree.Sdk;
using Client = Qweree.Authentication.WebApi.Domain.Identity.Client;
using ClientCreateInput = Qweree.Authentication.AdminSdk.Identity.Clients.ClientCreateInput;
using ClientModifyInput = Qweree.Authentication.AdminSdk.Identity.Clients.ClientModifyInput;
using IAccessDefinitionInput = Qweree.Authentication.AdminSdk.Identity.Clients.IAccessDefinitionInput;
using RolesCollection = Qweree.Authentication.AdminSdk.Authorization.Roles.RolesCollection;
using SdkClient = Qweree.Authentication.AdminSdk.Identity.Clients.Client;

namespace Qweree.Authentication.WebApi.Web.Admin.Identity;

[ApiController]
[Route("/api/admin/identity/clients")]
public class ClientController : ControllerBase
{
    private readonly ClientService _clientService;
    private readonly AdminSdkMapperService _sdkMapperService;

    public ClientController(ClientService clientService, AdminSdkMapperService sdkMapperService)
    {
        _clientService = clientService;
        _sdkMapperService = sdkMapperService;
    }

    /// <summary>
    ///     Create client.
    /// </summary>
    /// <param name="input">Create clint input.</param>
    /// <returns>Created client.</returns>
    [HttpPost]
    [Authorize(Policy = "ClientCreate")]
    [ProducesResponseType(typeof(ClientWithSecret), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ClientCreateActionAsync(ClientCreateInput input)
    {
        var serviceInput = ClientMapper.ToClientCreateInput(input);

        var clientResponse = await _clientService.ClientCreateAsync(serviceInput);

        if (clientResponse.Status != ResponseStatus.Ok)
            return clientResponse.ToErrorActionResult();

        var client = await _sdkMapperService.ToClientWithSecretAsync(clientResponse.Payload!);
        return Created($"/api/v1/clients/{client.Id}", client);
    }

    /// <summary>
    ///     Get client effective roles.
    /// </summary>
    /// <param name="id">Client id.</param>
    /// <returns>Found effective roles.</returns>
    [HttpGet("{id}/effective-roles")]
    [Authorize(Policy = "ClientRead")]
    [ProducesResponseType(typeof(RolesCollection), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ClientGetEffectiveRolesActionAsync(Guid id)
    {
        var rolesResponse = await _clientService.ClientGetEffectiveRolesAsync(id);

        if (rolesResponse.Status != ResponseStatus.Ok)
            return rolesResponse.ToErrorActionResult();

        var roles = _sdkMapperService.ToRolesCollection(rolesResponse.Payload!);
        return Ok(roles);
    }



    /// <summary>
    ///     Get client.
    /// </summary>
    /// <param name="id">Client id.</param>
    /// <returns>Client.</returns>
    [HttpGet]
    [Route("{id}")]
    [Authorize(Policy = "ClientRead")]
    [ProducesResponseType(typeof(Client), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ClientGetActionAsync(Guid id)
    {
        var clientResponse = await _clientService.ClientGetAsync(id);

        if (clientResponse.Status != ResponseStatus.Ok)
            return clientResponse.ToErrorActionResult();

        var client = await _sdkMapperService.ToClientAsync(clientResponse.Payload!);
        return Ok(client);
    }

    /// <summary>
    ///     Delete client.
    /// </summary>
    /// <param name="id">Client id.</param>
    [HttpDelete]
    [Route("{id}")]
    [Authorize(Policy = "ClientDelete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ClientDeleteActionAsync(Guid id)
    {
        var clientResponse = await _clientService.ClientDeleteAsync(id);

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
    [ProducesResponseType(typeof(List<Client>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ClientsPaginateActionAsync(
        [FromQuery(Name = "sort")] Dictionary<string, string[]> sort,
        [FromQuery(Name = "skip")] int skip = 0,
        [FromQuery(Name = "take")] int take = 50
    )
    {
        var sortDictionary = sort.ToDictionary(kv => kv.Key, kv => int.Parse(kv.Value.FirstOrDefault() ?? "1"));
        var clientsResponse = await _clientService.ClientPaginateAsync(skip, take, sortDictionary);

        if (clientsResponse.Status != ResponseStatus.Ok)
            return clientsResponse.ToErrorActionResult();

        var sortParts = sort.Select(s => $"sort[{s.Key}]={s.Value}");
        Response.Headers.AddLinkHeaders($"?{string.Join("&", sortParts)}", skip, take, clientsResponse.DocumentCount);

        Response.Headers.Add("q-document-count", new[] { clientsResponse.DocumentCount.ToString() });

        var clients = new List<SdkClient>();
        foreach (var document in clientsResponse.Payload ?? Array.Empty<Client>())
        {
            clients.Add(await _sdkMapperService.ToClientAsync(document));
        }

        return Ok(clients);
    }

    /// <summary>
    ///     Find client active sessions
    /// </summary>
    [HttpGet("{id}/sessions")]
    [Authorize(Policy = "ClientRead")]
    [ProducesResponseType(typeof(List<SessionInfo>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UsersFindActiveSessionsAction(Guid userId)
    {
        var sessionResponse = await _clientService.ClientGetActiveSessions(userId);

        if (sessionResponse.Status != ResponseStatus.Ok)
            return sessionResponse.ToErrorActionResult();

        return Ok(await _sdkMapperService.ToSessionInfosAsync(sessionResponse.Payload!));
    }

    /// <summary>
    ///     Modify client.
    /// </summary>
    /// <param name="id">Client id.</param>
    /// <param name="input">Modify client input.</param>
    /// <returns>Modified client.</returns>
    [HttpPatch("{id:guid}")]
    [Authorize(Policy = "ClientModify")]
    [ProducesResponseType(typeof(SdkClient), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ClientModifyActionAsync(Guid id, ClientModifyInput input)
    {
        var serviceInput = ClientMapper.ToClientModifyInput(input);

        var clientResponse = await _clientService.ClientModifyAsync(id, serviceInput);

        if (clientResponse.Status != ResponseStatus.Ok)
            return clientResponse.ToErrorActionResult();

        var client = await _sdkMapperService.ToClientAsync(clientResponse.Payload!);
        return Ok(client);
    }

    /// <summary>
    ///     Regenerate client secret.
    /// </summary>
    /// <param name="id">Client id.</param>
    /// <returns>Modified client.</returns>
    [HttpPost("{id:guid}/regenerate-secret")]
    [Authorize(Policy = "ClientModify")]
    [ProducesResponseType(typeof(ClientWithSecret), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ClientSecretRegenerateActionAsync(Guid id)
    {
        var clientResponse = await _clientService.ClientSecretRegenerateAsync(id);

        if (clientResponse.Status != ResponseStatus.Ok)
            return clientResponse.ToErrorActionResult();

        var client = await _sdkMapperService.ToClientWithSecretAsync(clientResponse.Payload!);
        return Ok(client);
    }

    /// <summary>
    ///     Set access definitions.
    /// </summary>
    /// <param name="id">Client id.</param>
    /// <param name="input">List of access definitions.</param>
    /// <returns>Modified client.</returns>
    [HttpPut("{id:guid}/access-definitions")]
    [Authorize(Policy = "ClientModify")]
    [ProducesResponseType(typeof(Client), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ClientSecretRegenerateActionAsync(Guid id, IEnumerable<IAccessDefinitionInput> input)
    {
        var clientResponse = await _clientService.AccessDefinitionsReplaceAsync(id, input.Select(ClientMapper.ToAccessDefinitionInput));

        if (clientResponse.Status != ResponseStatus.Ok)
            return clientResponse.ToErrorActionResult();

        var client = await _sdkMapperService.ToClientAsync(clientResponse.Payload!);
        return Ok(client);
    }
}