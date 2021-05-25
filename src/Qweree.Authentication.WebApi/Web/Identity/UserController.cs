using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Qweree.AspNet.Application;
using Qweree.AspNet.Web;
using Qweree.Authentication.AdminSdk.Identity.Users;
using Qweree.Authentication.WebApi.Domain.Identity;
using Qweree.Sdk;

namespace Qweree.Authentication.WebApi.Web.Identity
{
    [ApiController]
    [Route("/api/admin/identity/users")]
    public class UserController : ControllerBase
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly UserService _userService;

        public UserController(UserService userService, IAuthorizationService authorizationService)
        {
            _userService = userService;
            _authorizationService = authorizationService;
        }

        /// <summary>
        ///     Create user.
        /// </summary>
        /// <param name="input">Create user input.</param>
        /// <returns>Created user.</returns>
        [HttpPost]
        [Authorize(Policy = "UserCreate")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateUserActionAsync(UserCreateInputDto input)
        {
            var serviceInput = new UserCreateInput(input.Username ?? "", input.ContactEmail ?? "",
                input.FullName ?? "", input.Password ?? "", input.Roles ?? Array.Empty<Guid>());

            var userResponse = await _userService.CreateUserAsync(serviceInput);

            if (userResponse.Status != ResponseStatus.Ok)
                return BadRequest(userResponse.ToErrorResponseDto());

            var userDto = UserMapper.ToDto(userResponse.Payload!);
            return Created($"/api/v1/users/{userDto.Id}", userDto);
        }

        /// <summary>
        ///     Get user by id.
        /// </summary>
        /// <param name="id">User id.</param>
        /// <returns>Found user.</returns>
        [HttpGet("{id}")]
        [Authorize(Policy = "UserRead")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetUserActionAsync(Guid id)
        {
            var userResponse = await _userService.GetUserAsync(id);

            if (userResponse.Status != ResponseStatus.Ok)
                return userResponse.ToErrorActionResult();

            var userDto = UserMapper.ToDto(userResponse.Payload!);

            var result = await _authorizationService.AuthorizeAsync(User, null, "UserReadPersonalDetail");

            if (!result.Succeeded)
            {
                userDto.ContactEmail = "***";
                userDto.FullName = "***";
            }

            return Ok(userDto);
        }

        /// <summary>
        ///     Find users
        /// </summary>
        /// <param name="skip">How many items should lookup to database skip. Default: 0</param>
        /// <param name="take">How many items should be returned. Default: 100</param>
        /// <param name="sort">
        ///     Sorting.
        ///     Asc 1; Desc -1
        ///     sort[CreatedAt]=-1 // sort by created, desc.
        /// </param>
        /// <returns>Collection of users.</returns>
        [HttpGet]
        [Authorize(Policy = "UserRead")]
        [ProducesResponseType(typeof(List<UserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> FindUsersActionAsync(
            [FromQuery(Name = "sort")] Dictionary<string, string[]> sort,
            [FromQuery(Name = "skip")] int skip = 0,
            [FromQuery(Name = "take")] int take = 50
        )
        {
            var sortDictionary = sort.ToDictionary(kv => kv.Key, kv => int.Parse(kv.Value.FirstOrDefault() ?? "1"));
            var input = new FindUsersInput(skip, take, sortDictionary);
            var usersResponse = await _userService.FindUsersAsync(input);

            if (usersResponse.Status != ResponseStatus.Ok)
                return usersResponse.ToErrorActionResult();

            var sortParts = sort.Select(s => $"sort[{s.Key}]={s.Value}");
            Response.Headers.AddLinkHeaders($"?{string.Join("&", sortParts)}", skip, take, usersResponse.DocumentCount);

            var usersDto = usersResponse.Payload?.Select(UserMapper.ToDto);
            return Ok(usersDto);
        }

        /// <summary>
        ///     Delete user.
        /// </summary>
        /// <param name="id">Users identity.</param>
        /// <returns>Empty response.</returns>
        [HttpDelete("{id}")]
        [Authorize(Policy = "UserDelete")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteActionAsync(Guid id)
        {
            var deleteResponse = await _userService.DeleteAsync(id);

            if (deleteResponse.Status != ResponseStatus.Ok)
                return deleteResponse.ToErrorActionResult();

            return Ok(NoContent());
        }
    }
}