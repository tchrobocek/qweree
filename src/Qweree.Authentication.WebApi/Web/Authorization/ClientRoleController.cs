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

namespace Qweree.Authentication.WebApi.Web.Authorization
{
    [ApiController]
    [Route("/api/admin/authorization/clientRoles")]
    public class ClientRoleController : ControllerBase
    {
        private readonly RoleService _roleService;

        public ClientRoleController(RoleService roleService)
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
        [ProducesResponseType(typeof(ClientRoleDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateRoleActionAsync(ClientRoleCreateInputDto input)
        {
            var serviceInput = ClientRoleMapper.FromDto(input);

            var response = await _roleService.CreateClientRoleAsync(serviceInput);

            if (response.Status != ResponseStatus.Ok)
                return response.ToErrorActionResult();

            var payload = ClientRoleMapper.ToDto(response.Payload!);
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
        [ProducesResponseType(typeof(ClientRoleDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ModifyRoleActionAsync(Guid id, ClientRoleModifyInputDto input)
        {
            var serviceInput = ClientRoleMapper.FromDto(id, input);

            var response = await _roleService.ModifyClientRoleAsync(serviceInput);

            if (response.Status != ResponseStatus.Ok)
                return response.ToErrorActionResult();

            var payload = ClientRoleMapper.ToDto(response.Payload!);
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
        public async Task<IActionResult> DeleteRoleActionAsync(Guid id)
        {
            var response = await _roleService.DeleteClientRoleAsync(id);

            if (response.Status != ResponseStatus.Ok)
                return response.ToErrorActionResult();

            return NoContent();
        }

        /// <summary>
        ///     Get roles.
        /// </summary>
        [HttpGet()]
        [Authorize(Policy = "RoleRead")]
        [ProducesResponseType(typeof(ClientRoleDto[]), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetRolesActionAsync()
        {
            var response = await _roleService.FindClientRolesAsync();

            if (response.Status != ResponseStatus.Ok)
                return response.ToErrorActionResult();

            return Ok(response.Payload!.Select(ClientRoleMapper.ToDto));
        }
    }
}