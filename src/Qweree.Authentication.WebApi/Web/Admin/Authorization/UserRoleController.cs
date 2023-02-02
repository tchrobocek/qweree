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
using UserRole = Qweree.Authentication.AdminSdk.Authorization.Roles.UserRole;
using UserRoleCreateInput = Qweree.Authentication.AdminSdk.Authorization.Roles.UserRoleCreateInput;
using UserRoleModifyInput = Qweree.Authentication.AdminSdk.Authorization.Roles.UserRoleModifyInput;

namespace Qweree.Authentication.WebApi.Web.Admin.Authorization;

[ApiController]
[Route("/api/admin/authorization/user-roles")]
public class UserRoleController : ControllerBase
{
    private readonly RoleService _roleService;
    private readonly AdminSdkMapperService _sdkMapperService;

    public UserRoleController(RoleService roleService, AdminSdkMapperService sdkMapperService)
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
    [ProducesResponseType(typeof(UserRole), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UserRoleCreateActionAsync(UserRoleCreateInput input)
    {
        var serviceInput = _sdkMapperService.ToUserRoleCreateInput(input);

        var response = await _roleService.UserRoleCreateAsync(serviceInput);

        if (response.Status != ResponseStatus.Ok)
            return response.ToErrorActionResult();

        var payload = await _sdkMapperService.ToUserRoleAsync(response.Payload!);
        return Created($"/api/admin/authorization/userRoles", payload);
    }

    /// <summary>
    ///     Modify role.
    /// </summary>
    /// <param name="id">Role id.</param>
    /// <param name="input">Modify role input.</param>
    /// <returns>Modified role.</returns>
    [HttpPatch("{id}")]
    [Authorize(Policy = "RoleModify")]
    [ProducesResponseType(typeof(UserRole), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UserRoleModifyActionAsync(Guid id, UserRoleModifyInput input)
    {
        var serviceInput = _sdkMapperService.ToUserRoleModifyInput(id, input);
        var response = await _roleService.UserRoleModifyAsync(serviceInput);

        if (response.Status != ResponseStatus.Ok)
            return response.ToErrorActionResult();

        var payload = await _sdkMapperService.ToUserRoleAsync(response.Payload!);
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
    public async Task<IActionResult> UserRoleDeleteActionAsync(Guid id)
    {
        var response = await _roleService.UserRoleDeleteAsync(id);

        if (response.Status != ResponseStatus.Ok)
            return response.ToErrorActionResult();

        return NoContent();
    }

    /// <summary>
    ///     Get roles.
    /// </summary>
    [HttpGet()]
    [Authorize(Policy = "RoleRead")]
    [ProducesResponseType(typeof(UserRole[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UserRolesFindAsync()
    {
        var response = await _roleService.UserRolesFindAsync();

        if (response.Status != ResponseStatus.Ok)
            return response.ToErrorActionResult();

        var roles = new List<UserRole>();
        foreach (var item in response.Payload!)
            roles.Add(await _sdkMapperService.ToUserRoleAsync(item));

        return Ok(roles);
    }
}