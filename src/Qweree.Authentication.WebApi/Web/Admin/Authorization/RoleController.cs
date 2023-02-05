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
using Role = Qweree.Authentication.AdminSdk.Authorization.Roles.Role;
using RoleCreateInput = Qweree.Authentication.AdminSdk.Authorization.Roles.RoleCreateInput;
using RoleModifyInput = Qweree.Authentication.AdminSdk.Authorization.Roles.RoleModifyInput;

namespace Qweree.Authentication.WebApi.Web.Admin.Authorization;

[ApiController]
[Route("/api/admin/authorization/roles")]
public class RoleController : ControllerBase
{
    private readonly RoleService _roleService;
    private readonly AdminSdkMapperService _sdkMapperService;

    public RoleController(RoleService roleService, AdminSdkMapperService sdkMapperService)
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
    [ProducesResponseType(typeof(Role), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RoleCreateActionAsync(RoleCreateInput input)
    {
        var serviceInput = _sdkMapperService.ToRoleCreateInput(input);

        var response = await _roleService.RoleCreateAsync(serviceInput);

        if (response.Status != ResponseStatus.Ok)
            return response.ToErrorActionResult();

        var payload = await _sdkMapperService.ToRoleAsync(response.Payload!);
        return Created($"/api/admin/authorization/roles", payload);
    }

    /// <summary>
    ///     Modify role.
    /// </summary>
    /// <param name="id">Role id.</param>
    /// <param name="input">Modify role input.</param>
    /// <returns>Modified role.</returns>
    [HttpPatch("{id}")]
    [Authorize(Policy = "RoleModify")]
    [ProducesResponseType(typeof(Role), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RoleModifyActionAsync(Guid id, RoleModifyInput input)
    {
        var serviceInput = _sdkMapperService.ToRoleModifyInput(id, input);
        var response = await _roleService.RoleModifyAsync(serviceInput);

        if (response.Status != ResponseStatus.Ok)
            return response.ToErrorActionResult();

        var payload = await _sdkMapperService.ToRoleAsync(response.Payload!);
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
    public async Task<IActionResult> RoleDeleteActionAsync(Guid id)
    {
        var response = await _roleService.RoleDeleteAsync(id);

        if (response.Status != ResponseStatus.Ok)
            return response.ToErrorActionResult();

        return NoContent();
    }

    /// <summary>
    ///     Get roles.
    /// </summary>
    [HttpGet()]
    [Authorize(Policy = "RoleRead")]
    [ProducesResponseType(typeof(Role[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RolesFindAsync()
    {
        var response = await _roleService.RolesFindAsync();

        if (response.Status != ResponseStatus.Ok)
            return response.ToErrorActionResult();

        var roles = new List<Role>();
        foreach (var item in response.Payload!)
            roles.Add(await _sdkMapperService.ToRoleAsync(item));

        return Ok(roles);
    }
}