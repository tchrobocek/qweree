using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Qweree.AspNet.Application;
using Qweree.AspNet.Web;
using Qweree.Authentication.Sdk.Account;
using Qweree.Authentication.Sdk.Session;
using Qweree.Sdk;

namespace Qweree.Authentication.WebApi.Web.Account;

[ApiController]
[Authorize]
[Route("/api/account")]
public class MyAccountController : ControllerBase
{
    private readonly Domain.Account.MyAccountService _myAccountService;

    public MyAccountController(Domain.Account.MyAccountService myAccountService)
    {
        _myAccountService = myAccountService;
    }

    /// <summary>
    ///     Change my password.
    /// </summary>
    /// <param name="input">Change my password input.</param>
    [HttpPost("change-password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangePasswordActionAsync(ChangeMyPasswordInputDto input)
    {
        var response = await _myAccountService.ChangeMyPasswordAsync(new ChangeMyPasswordInput(input.OldPassword ?? string.Empty, input.NewPassword ?? string.Empty));

        if (response.Status != ResponseStatus.Ok)
            return response.ToErrorActionResult();

        return NoContent();
    }

    /// <summary>
    ///     Find my devices.
    /// </summary>
    [HttpGet("my-devices")]
    [ProducesResponseType(typeof(DeviceInfoDto[]), StatusCodes.Status200OK)]
    public async Task<IActionResult> MyDevicesGetActionAsync()
    {
        var response = await _myAccountService.FindMyDevicesAsync();

        if (response.Status != ResponseStatus.Ok)
            return response.ToErrorActionResult();

        return Ok(response.Payload!.Select(DeviceInfoMapper.ToDto));
    }

    /// <summary>
    ///     Get my account.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IdentityUserDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> MyProfileGetActionAsync()
    {
        var response = await _myAccountService.GetMeAsync();

        if (response.Status != ResponseStatus.Ok)
            return response.ToErrorActionResult();

        return Ok(MyProfileMapper.ToDto(response.Payload!));
    }
}