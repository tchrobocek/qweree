using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Qweree.AspNet.Application;
using Qweree.AspNet.Web;
using Qweree.Authentication.AdminSdk.Authorization.Roles;
using Qweree.Authentication.Sdk.Identity;
using Qweree.Authentication.WebApi.Domain.Identity;
using Qweree.Authentication.WebApi.Infrastructure;
using Qweree.Sdk;
using SdkUser = Qweree.Authentication.AdminSdk.Identity.Users.User;

namespace Qweree.Authentication.WebApi.Web.Admin.Identity;

[ApiController]
[Route("/api/admin/identity/users")]
public class UserController : ControllerBase
{
    private readonly IAuthorizationService _authorizationService;
    private readonly UserService _userService;
    private readonly AdminSdkMapperService _sdkMapperService;

    public UserController(UserService userService, IAuthorizationService authorizationService, AdminSdkMapperService sdkMapperService)
    {
        _userService = userService;
        _authorizationService = authorizationService;
        _sdkMapperService = sdkMapperService;
    }

    /// <summary>
    ///     Get user by id.
    /// </summary>
    /// <param name="id">User id.</param>
    /// <returns>Found user.</returns>
    [HttpGet("{id}")]
    [Authorize(Policy = "UserRead")]
    [ProducesResponseType(typeof(SdkUser), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UserGetActionAsync(Guid id)
    {
        var userResponse = await _userService.UserGetAsync(id);

        if (userResponse.Status != ResponseStatus.Ok)
            return userResponse.ToErrorActionResult();

        var user = await _sdkMapperService.ToUserAsync(userResponse.Payload!);
        var result = await _authorizationService.AuthorizeAsync(User, null, "UserReadPersonalDetail");

        if (!result.Succeeded)
        {
            user.ContactEmail = "***";
            var fullNameProperty = user.Properties?.FirstOrDefault(p => p.Key == UserProperties.FullName);
            if (fullNameProperty != null)
                fullNameProperty.Value = "***";
        }

        return Ok(user);
    }

    /// <summary>
    ///     Get user effective roles.
    /// </summary>
    /// <param name="id">User id.</param>
    /// <returns>Found effective roles.</returns>
    [HttpGet("{id}/effective-roles")]
    [Authorize(Policy = "UserRead")]
    [ProducesResponseType(typeof(List<Role>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UserGetEffectiveRolesActionAsync(Guid id)
    {
        var rolesResponse = await _userService.UserGetEffectiveRolesAsync(id);

        if (rolesResponse.Status != ResponseStatus.Ok)
            return rolesResponse.ToErrorActionResult();

        return Ok(rolesResponse.Payload!.Select(r => _sdkMapperService.ToRole(r)));
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
    [ProducesResponseType(typeof(List<SdkUser>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UsersPaginateActionAsync(
        [FromQuery(Name = "sort")] Dictionary<string, string[]> sort,
        [FromQuery(Name = "skip")] int skip = 0,
        [FromQuery(Name = "take")] int take = 50
    )
    {
        var sortDictionary = sort.ToDictionary(kv => kv.Key, kv => int.Parse(kv.Value.FirstOrDefault() ?? "1"));
        var input = new UserFindInput(skip, take, sortDictionary);
        var usersResponse = await _userService.UsersPaginateAsync(input);

        if (usersResponse.Status != ResponseStatus.Ok)
            return usersResponse.ToErrorActionResult();

        var sortParts = sort.Select(s => $"sort[{s.Key}]={s.Value}");
        Response.Headers.AddLinkHeaders($"?{string.Join("&", sortParts)}", skip, take, usersResponse.DocumentCount);

        Response.Headers.Add("q-document-count", new[] { usersResponse.DocumentCount.ToString() });

        var users = new List<SdkUser>();
        foreach (var user in usersResponse.Payload ?? Array.Empty<User>())
            users.Add(await _sdkMapperService.ToUserAsync(user));
        return Ok(users);
    }

    /// <summary>
    ///     Delete user.
    /// </summary>
    /// <param name="id">Users identity.</param>
    /// <returns>Empty response.</returns>
    [HttpDelete("{id}")]
    [Authorize(Policy = "UserDelete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UserDeleteActionAsync(Guid id)
    {
        var deleteResponse = await _userService.UserDeleteAsync(id);

        if (deleteResponse.Status != ResponseStatus.Ok)
            return deleteResponse.ToErrorActionResult();

        return Ok(NoContent());
    }
}