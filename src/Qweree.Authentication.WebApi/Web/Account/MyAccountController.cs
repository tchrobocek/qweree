using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Qweree.AspNet.Application;
using Qweree.AspNet.Web;
using Qweree.Authentication.Sdk.Account.MyAccount;
using Qweree.Authentication.Sdk.Session;
using Qweree.Authentication.WebApi.Domain.Account;
using Qweree.Authentication.WebApi.Infrastructure;
using Qweree.Authentication.WebApi.Infrastructure.Account;
using Qweree.Sdk;
using ChangeMyPasswordInput = Qweree.Authentication.WebApi.Domain.Account.ChangeMyPasswordInput;
using SdkChangeMyPasswordInput = Qweree.Authentication.Sdk.Account.MyAccount.ChangeMyPasswordInput;

namespace Qweree.Authentication.WebApi.Web.Account;

[ApiController]
[Authorize]
[Route("/api/account")]
public class MyAccountController : ControllerBase
{
    private readonly MyAccountService _myAccountService;
    private readonly AuthSdkMapperService _authSdkMapper;

    public MyAccountController(MyAccountService myAccountService, AuthSdkMapperService authSdkMapper)
    {
        _myAccountService = myAccountService;
        _authSdkMapper = authSdkMapper;
    }

    /// <summary>
    ///     Change my password.
    /// </summary>
    /// <param name="input">Change my password input.</param>
    [HttpPost("change-password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangePasswordActionAsync(SdkChangeMyPasswordInput input)
    {
        var response = await _myAccountService.ChangeMyPasswordAsync(new ChangeMyPasswordInput(input.OldPassword ?? string.Empty, input.NewPassword ?? string.Empty));

        if (response.Status != ResponseStatus.Ok)
            return response.ToErrorActionResult();

        return NoContent();
    }

    /// <summary>
    ///     Get my account.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IdentityUser), StatusCodes.Status200OK)]
    public async Task<IActionResult> MyProfileGetActionAsync()
    {
        var response = await _myAccountService.GetMeAsync();

        if (response.Status != ResponseStatus.Ok)
            return response.ToErrorActionResult();

        return Ok(MyProfileMapper.Map(response.Payload!));
    }

    /// <summary>
    ///     Get my sessions.
    /// </summary>
    [HttpGet("sessions")]
    [ProducesResponseType(typeof(SessionInfo[]), StatusCodes.Status200OK)]
    public async Task<IActionResult> MySessionsGetActionAsync()
    {
        var response = await _myAccountService.FindMySessions();

        if (response.Status != ResponseStatus.Ok)
            return response.ToErrorActionResult();

        var infos = new List<SessionInfo>();
        foreach (var item in response.Payload!)
            infos.Add(await _authSdkMapper.ToSessionInfoAsync(item));

        return Ok(infos);
    }
}