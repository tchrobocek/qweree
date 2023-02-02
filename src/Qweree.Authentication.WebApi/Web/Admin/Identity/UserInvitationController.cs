using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Qweree.AspNet.Application;
using Qweree.AspNet.Web;
using Qweree.Authentication.AdminSdk.Identity.Users.UserInvitation;
using Qweree.Authentication.WebApi.Domain.Identity.UserInvitation;
using Qweree.Authentication.WebApi.Infrastructure;
using Qweree.Sdk;
using UserInvitationInput = Qweree.Authentication.AdminSdk.Identity.Users.UserInvitation.UserInvitationInput;

namespace Qweree.Authentication.WebApi.Web.Admin.Identity;

[ApiController]
[Route("/api/admin/identity/user-invitations")]
public class UserInvitationController : ControllerBase
{
    private readonly UserInvitationService _userInvitationService;
    private readonly AdminSdkMapperService _sdkMapper;

    public UserInvitationController(UserInvitationService userInvitationService, AdminSdkMapperService sdkMapper)
    {
        _userInvitationService = userInvitationService;
        _sdkMapper = sdkMapper;
    }

    /// <summary>
    ///     Create user invitation.
    /// </summary>
    /// <param name="userInvitation">User invitation.</param>
    /// <returns>Created user invitation.</returns>
    [HttpPost]
    [Authorize(Policy = "UserInvitationCreate")]
    [ProducesResponseType(typeof(UserInvitation), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UserInvitationCreateActionAsync(UserInvitationInput userInvitation)
    {
        var input = _sdkMapper.ToUserInvitation(userInvitation);
        var userInvitationResponse = await _userInvitationService.UserInvitationCreateAsync(input);

        if (userInvitationResponse.Status != ResponseStatus.Ok)
            return userInvitationResponse.ToErrorActionResult();

        var invitation = UserInvitationDescriptorMapper.ToUserInvitation(userInvitationResponse.Payload!);

        var uri = new Uri($"{Request.Scheme}://{Request.Host}/api/admin/identity/user-invitations/{invitation.Id}");
        return Created(uri, invitation);
    }

    /// <summary>
    ///     Get user invitation by id.
    /// </summary>
    /// <param name="id">User invitation id.</param>
    /// <returns>Found user invitation.</returns>
    [HttpGet("{id}")]
    [Authorize(Policy = "UserInvitationRead")]
    [ProducesResponseType(typeof(UserInvitation), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UserInvitationGetActionAsync(Guid id)
    {
        var userInvitationResponse = await _userInvitationService.UserInvitationGetAsync(id);

        if (userInvitationResponse.Status != ResponseStatus.Ok)
            return userInvitationResponse.ToErrorActionResult();

        var invitation = UserInvitationDescriptorMapper.ToUserInvitation(userInvitationResponse.Payload!);

        return Ok(invitation);
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
    [ProducesResponseType(typeof(List<UserInvitation>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
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

        var users = userInvitationsResponse.Payload?.Select(UserInvitationDescriptorMapper.ToUserInvitation);
        return Ok(users);
    }

    /// <summary>
    ///     Delete user invitation.
    /// </summary>
    /// <param name="id">User invitation identity.</param>
    /// <returns>Empty response.</returns>
    [HttpDelete("{id}")]
    [Authorize(Policy = "UserInvitationDelete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UserDeleteActionAsync(Guid id)
    {
        var deleteResponse = await _userInvitationService.UserInvitationDeleteAsync(id);

        if (deleteResponse.Status != ResponseStatus.Ok)
            return deleteResponse.ToErrorActionResult();

        return Ok(NoContent());
    }
}