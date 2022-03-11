using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Qweree.AspNet.Application;
using Qweree.AspNet.Web;
using Qweree.Authentication.Sdk.Account;
using Qweree.Authentication.WebApi.Domain.Account;
using Qweree.Sdk;

namespace Qweree.Authentication.WebApi.Web.Account;

[ApiController]
[Route("/api/account")]
public class MyAccountService : ControllerBase
{
    private readonly Domain.Account.MyAccountService _myAccountService;

    public MyAccountService(Domain.Account.MyAccountService myAccountService)
    {
        _myAccountService = myAccountService;
    }

    /// <summary>
    ///     Change my password.
    /// </summary>
    /// <param name="input">Change my password input.</param>
    [HttpPost("change-password")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ClientCreateActionAsync(ChangeMyPasswordInputDto input)
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
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> MyDevicesFindActionAsync()
    {
        var response = await _myAccountService.FindMyDevicesAsync();

        if (response.Status != ResponseStatus.Ok)
            return response.ToErrorActionResult();

        return Ok(response.Payload!.Select(DeviceInfoMapper.ToDto));
    }
}