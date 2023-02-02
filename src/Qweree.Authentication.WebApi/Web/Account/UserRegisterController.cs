using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Qweree.AspNet.Application;
using Qweree.AspNet.Web;
using Qweree.Authentication.Sdk.Account;
using Qweree.Authentication.WebApi.Domain.Identity.UserInvitation;
using Qweree.Authentication.WebApi.Infrastructure.Account;
using Qweree.Sdk;

namespace Qweree.Authentication.WebApi.Web.Account;

[ApiController]
[Route("/api/account/register")]
public class UserRegisterController : ControllerBase
{
    private readonly UserRegisterService _userRegisterService;
    private readonly UserInvitationService _userInvitationService;

    public UserRegisterController(UserRegisterService userRegisterService, UserInvitationService userInvitationService)
    {
        _userRegisterService = userRegisterService;
        _userInvitationService = userInvitationService;
    }

    /// <summary>
    ///     Register.
    /// </summary>
    /// <param name="input">User register input.</param>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UserRegisterActionAsync(UserRegisterInput input)
    {
        var response = await _userRegisterService.RegisterAsync(UserRegisterInputMapper.Map(input));

        if (response.Status != ResponseStatus.Ok)
            return response.ToErrorActionResult();

        return NoContent();
    }

    /// <summary>
    ///     Returns user invitation.
    /// </summary>
    /// <param name="id">User invitation id.</param>
    [HttpGet("invitation/{id}")]
    [ProducesResponseType(typeof(UserInvitation), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UserInvitationGetActionAsync(Guid id)
    {
        var response = await _userInvitationService.UserInvitationGetAsync(id);

        if (response.Status != ResponseStatus.Ok)
            return response.ToErrorActionResult();

        return Ok(new UserInvitation
        {
            Id = response.Payload!.Id,
            Username = response.Payload.Username,
            ContactEmail = response.Payload.ContactEmail,
            ExpiresAt = response.Payload.ExpiresAt,
            FullName = response.Payload.FullName,
        });
    }
}