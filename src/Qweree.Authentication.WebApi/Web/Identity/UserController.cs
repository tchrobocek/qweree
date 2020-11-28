using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Qweree.AspNet.Application;
using Qweree.AspNet.Web;
using Qweree.Authentication.Sdk.Identity;
using Qweree.Authentication.WebApi.Application.Identity;
using Qweree.Sdk;
using User = Qweree.Authentication.WebApi.Domain.Identity.User;

namespace Qweree.Authentication.WebApi.Web.Identity
{
    [ApiController]
    [Route("/api/v1/identity/users")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Create user.
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
                input.FullName ?? "", input.Password ?? "", input.Roles ?? Array.Empty<string>());

            var userResponse = await _userService.CreateUserAsync(serviceInput);

            if (userResponse.Status != ResponseStatus.Ok)
            {
                return BadRequest(userResponse.ToErrorResponseDto());
            }

            var userDto = UserToDto(userResponse.Payload ?? throw new InvalidOperationException("Empty payload."));
            return Created("", userDto);
        }

        private UserDto UserToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Username = user.Username
            };
        }
    }
}