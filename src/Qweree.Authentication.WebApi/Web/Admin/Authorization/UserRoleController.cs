using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Qweree.AspNet.Application;
using Qweree.AspNet.Web;
using Qweree.Authentication.AdminSdk.Authorization.Roles;
using Qweree.Authentication.WebApi.Domain.Authorization.Roles;
using Qweree.Sdk;

namespace Qweree.Authentication.WebApi.Web.Admin.Authorization;

[ApiController]
[Route("/api/admin/authorization/user-roles")]
public class UserRoleController : ControllerBase
{
    private readonly RoleService _roleService;

    public UserRoleController(RoleService roleService)
    {
        _roleService = roleService;
    }

    /// <summary>
    ///     Create role.
    /// </summary>
    /// <param name="input">Create role input.</param>
    /// <returns>Created role.</returns>
    [HttpPost]
    [Authorize(Policy = "RoleCreate")]
    [ProducesResponseType(typeof(UserRoleDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UserRoleCreateActionAsync(UserRoleCreateInputDto input)
    {
        var serviceInput = UserRoleMapper.FromDto(input);

        var response = await _roleService.UserRoleCreateAsync(serviceInput);

        if (response.Status != ResponseStatus.Ok)
            return response.ToErrorActionResult();

        var payload = UserRoleMapper.ToDto(response.Payload!);
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
    [ProducesResponseType(typeof(UserRoleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UserRoleModifyActionAsync(Guid id, UserRoleModifyInputDto input)
    {
        var serviceInput = UserRoleMapper.FromDto(id, input);

        var response = await _roleService.UserRoleModifyAsync(serviceInput);

        if (response.Status != ResponseStatus.Ok)
            return response.ToErrorActionResult();

        var payload = UserRoleMapper.ToDto(response.Payload!);
        return Ok(payload);
    }

    /// <summary>
    ///     Delete role.
    /// </summary>
    /// <param name="id">Role id.</param>
    [HttpDelete("{id}")]
    [Authorize(Policy = "RoleDelete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
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
    [ProducesResponseType(typeof(UserRoleDto[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UserRolesFindActionAsync()
    {
        var response = await _roleService.UserRolesFindAsync();

        if (response.Status != ResponseStatus.Ok)
            return response.ToErrorActionResult();

        return Ok(response.Payload!.Select(UserRoleMapper.ToDto));
    }
}