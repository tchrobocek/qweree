using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Qweree.AspNet.Application;
using Qweree.AspNet.Web;
using Qweree.Authentication.Sdk.Account;
using Qweree.Authentication.WebApi.Domain.Account;

namespace Qweree.Authentication.WebApi.Web.Account;

[ApiController]
[Route("/api/account/privacy")]
public class PrivacyController : ControllerBase
{
    private readonly PrivacyService _privacyService;

    public PrivacyController(PrivacyService privacyService)
    {
        _privacyService = privacyService;
    }

    /// <summary>
    ///     Find my devices.
    /// </summary>
    [HttpGet("my-devices")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> MyDevicesFindActionAsync()
    {
        var response = await _privacyService.FindMyDevicesAsync();

        if (response.Status != ResponseStatus.Ok)
            return response.ToErrorActionResult();

        return Ok(response.Payload!.Select(DeviceInfoMapper.ToDto));
    }
}