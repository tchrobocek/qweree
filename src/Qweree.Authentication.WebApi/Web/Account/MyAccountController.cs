using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Qweree.AspNet.Application;
using Qweree.AspNet.Web;
using Qweree.Authentication.Sdk.Session;
using Qweree.Authentication.WebApi.Domain.Account;
using Qweree.Authentication.WebApi.Infrastructure.Account;
using Qweree.Sdk;
using ChangeMyPasswordInput = Qweree.Authentication.Sdk.Account.ChangeMyPasswordInput;
using DeviceInfo = Qweree.Authentication.Sdk.Account.DeviceInfo;

namespace Qweree.Authentication.WebApi.Web.Account;

[ApiController]
[Authorize]
[Route("/api/account")]
public class MyAccountController : ControllerBase
{
    private readonly MyAccountService _myAccountService;

    public MyAccountController(MyAccountService myAccountService)
    {
        _myAccountService = myAccountService;
    }

    /// <summary>
    ///     Change my password.
    /// </summary>
    /// <param name="input">Change my password input.</param>
    [HttpPost("change-password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangePasswordActionAsync(ChangeMyPasswordInput input)
    {
        var response = await _myAccountService.ChangeMyPasswordAsync(new Domain.Account.ChangeMyPasswordInput(input.OldPassword ?? string.Empty, input.NewPassword ?? string.Empty));

        if (response.Status != ResponseStatus.Ok)
            return response.ToErrorActionResult();

        return NoContent();
    }

    /// <summary>
    ///     Find my devices.
    /// </summary>
    [HttpGet("my-devices")]
    [ProducesResponseType(typeof(DeviceInfo[]), StatusCodes.Status200OK)]
    public async Task<IActionResult> MyDevicesGetActionAsync()
    {
        var response = await _myAccountService.FindMyDevicesAsync();

        if (response.Status != ResponseStatus.Ok)
            return response.ToErrorActionResult();

        return Ok(response.Payload!.Select(DeviceInfoMapper.Map));
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
}