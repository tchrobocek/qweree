using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Qweree.AspNet.Application;
using Qweree.AspNet.Web;
using Qweree.Authentication.WebApi.Domain.Authorization.Roles;
using Qweree.Authentication.WebApi.Infrastructure;
using Qweree.Sdk;
using ClientRole = Qweree.Authentication.AdminSdk.Authorization.Roles.ClientRole;
using ClientRoleCreateInput = Qweree.Authentication.AdminSdk.Authorization.Roles.ClientRoleCreateInput;
using ClientRoleModifyInput = Qweree.Authentication.AdminSdk.Authorization.Roles.ClientRoleModifyInput;

namespace Qweree.Authentication.WebApi.Web.Admin.Authorization;

[ApiController]
[Route("/api/admin/authorization/client-roles")]
public class ClientRoleController : ControllerBase
{
    private readonly RoleService _roleService;
    private readonly SdkMapperService _sdkMapperService;

    public ClientRoleController(RoleService roleService, SdkMapperService sdkMapperService)
    {
        _roleService = roleService;
        _sdkMapperService = sdkMapperService;
    }

    /// <summary>
    ///     Create role.
    /// </summary>
    /// <param name="input">Create role input.</param>
    /// <returns>Created role.</returns>
    [HttpPost]
    [Authorize(Policy = "RoleCreate")]
    [ProducesResponseType(typeof(ClientRole), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ClientRoleCreateActionAsync(ClientRoleCreateInput input)
    {
        var serviceInput = _sdkMapperService.ToClientRoleCreateInput(input);

        var response = await _roleService.ClientRoleCreateAsync(serviceInput);

        if (response.Status != ResponseStatus.Ok)
            return response.ToErrorActionResult();

        var payload = await _sdkMapperService.ToClientRoleAsync(response.Payload!);
        return Created($"/api/admin/authorization/clientRoles", payload);
    }

    /// <summary>
    ///     Modify role.
    /// </summary>
    /// <param name="id">Role id.</param>
    /// <param name="input">Modify role input.</param>
    /// <returns>Modified role.</returns>
    [HttpPatch("{id}")]
    [Authorize(Policy = "RoleModify")]
    [ProducesResponseType(typeof(ClientRole), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ClientRoleModifyActionAsync(Guid id, ClientRoleModifyInput input)
    {
        var serviceInput = _sdkMapperService.ToClientRoleModifyInput(id, input);
        var response = await _roleService.ClientRoleModifyAsync(serviceInput);

        if (response.Status != ResponseStatus.Ok)
            return response.ToErrorActionResult();

        var payload = await _sdkMapperService.ToClientRoleAsync(response.Payload!);
        return Ok(payload);
    }

    /// <summary>
    ///     Delete role.
    /// </summary>
    /// <param name="id">Role id.</param>
    [HttpDelete("{id}")]
    [Authorize(Policy = "RoleDelete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ClientRoleDeleteActionAsync(Guid id)
    {
        var response = await _roleService.ClientRoleDeleteAsync(id);

        if (response.Status != ResponseStatus.Ok)
            return response.ToErrorActionResult();

        return NoContent();
    }

    /// <summary>
    ///     Get roles.
    /// </summary>
    [HttpGet()]
    [Authorize(Policy = "RoleRead")]
    [ProducesResponseType(typeof(ClientRole[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ClientRolesFindAsync()
    {
        var response = await _roleService.ClientRolesFindAsync();

        if (response.Status != ResponseStatus.Ok)
            return response.ToErrorActionResult();

        var roles = new List<ClientRole>();
        foreach (var item in response.Payload!)
            roles.Add(await _sdkMapperService.ToClientRoleAsync(item));

        return Ok(roles);
    }
}