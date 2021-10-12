using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Qweree.AspNet.Application;
using Qweree.AspNet.Web;
using Qweree.Authentication.AdminSdk.Identity.Users.UserRegister;
using Qweree.Authentication.WebApi.Infrastructure.Identity.UserRegister;
using Qweree.Sdk;
using UserInvitationMapper = Qweree.Authentication.AdminSdk.Identity.Users.UserRegister.UserInvitationMapper;

namespace Qweree.Authentication.WebApi.Web.Identity
{
    [ApiController]
    [Microsoft.AspNetCore.Components.Route("/api/admin/identity/user-invitations")]
    public class UserInvitationController : ControllerBase
    {
        private readonly UserInvitationService _userInvitationService;

        public UserInvitationController(UserInvitationService userInvitationService)
        {
            _userInvitationService = userInvitationService;
        }

        /// <summary>
        ///     Get user invitation by id.
        /// </summary>
        /// <param name="id">User invitation id.</param>
        /// <returns>Found user invitation.</returns>
        [HttpGet("{id}")]
        [Authorize(Policy = "UserInvitationRead")]
        [ProducesResponseType(typeof(UserInvitationDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UserInvitationGetActionAsync(Guid id)
        {
            var userInvitationResponse = await _userInvitationService.UserInvitationGetAsync(id);

            if (userInvitationResponse.Status != ResponseStatus.Ok)
                return userInvitationResponse.ToErrorActionResult();

            var userInvitationDto = UserInvitationMapper.ToDto(userInvitationResponse.Payload!);

            return Ok(userInvitationDto);
        }

        /// <summary>
        ///     Find user invitations
        /// </summary>
        /// <param name="skip">How many items should lookup to database skip. Default: 0</param>
        /// <param name="take">How many items should be returned. Default: 100</param>
        /// <param name="sort">
        ///     Sorting.
        ///     Asc 1; Desc -1
        ///     sort[CreatedAt]=-1 // sort by created, desc.
        /// </param>
        /// <returns>Collection of user invitations.</returns>
        [HttpGet]
        [Authorize(Policy = "UserInvitationRead")]
        [ProducesResponseType(typeof(List<UserInvitationDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UserInvitationsPaginateActionAsync(
            [FromQuery(Name = "sort")] Dictionary<string, string[]> sort,
            [FromQuery(Name = "skip")] int skip = 0,
            [FromQuery(Name = "take")] int take = 50
        )
        {
            var sortDictionary = sort.ToDictionary(kv => kv.Key, kv => int.Parse(kv.Value.FirstOrDefault() ?? "1"));
            var input = new UserInvitationsFindInput(skip, take, sortDictionary);
            var userInvitationsResponse = await _userInvitationService.UserInvitationsPaginateAsync(input);

            if (userInvitationsResponse.Status != ResponseStatus.Ok)
                return userInvitationsResponse.ToErrorActionResult();

            var sortParts = sort.Select(s => $"sort[{s.Key}]={s.Value}");
            Response.Headers.AddLinkHeaders($"?{string.Join("&", sortParts)}", skip, take, userInvitationsResponse.DocumentCount);

            Response.Headers.Add("q-document-count", new[] { userInvitationsResponse.DocumentCount.ToString() });

            var usersDto = userInvitationsResponse.Payload?.Select(UserInvitationMapper.ToDto);
            return Ok(usersDto);
        }

        /// <summary>
        ///     Delete user invitation.
        /// </summary>
        /// <param name="id">User invitation identity.</param>
        /// <returns>Empty response.</returns>
        [HttpDelete("{id}")]
        [Authorize(Policy = "UserInvitationDelete")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UserDeleteActionAsync(Guid id)
        {
            var deleteResponse = await _userInvitationService.UserInvitationDeleteAsync(id);

            if (deleteResponse.Status != ResponseStatus.Ok)
                return deleteResponse.ToErrorActionResult();

            return Ok(NoContent());
        }
    }
}