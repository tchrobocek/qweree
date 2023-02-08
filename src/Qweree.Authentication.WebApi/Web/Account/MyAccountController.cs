using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Qweree.AspNet.Application;
using Qweree.AspNet.Web;
using Qweree.Authentication.Sdk.Account.MyAccount;
using Qweree.Authentication.Sdk.Identity;
using Qweree.Authentication.Sdk.OAuth2;
using Qweree.Authentication.Sdk.Session;
using Qweree.Authentication.WebApi.Domain.Account;
using Qweree.Authentication.WebApi.Domain.Session;
using Qweree.Authentication.WebApi.Infrastructure;
using Qweree.Authentication.WebApi.Infrastructure.Session;
using Qweree.Sdk;
using ChangeMyPasswordInput = Qweree.Authentication.WebApi.Domain.Account.ChangeMyPasswordInput;
using IdentityMapper = Qweree.Authentication.Sdk.Session.IdentityMapper;
using SdkChangeMyPasswordInput = Qweree.Authentication.Sdk.Account.MyAccount.ChangeMyPasswordInput;

namespace Qweree.Authentication.WebApi.Web.Account;

[ApiController]
[Authorize]
[Route("/api/account")]
public class MyAccountController : ControllerBase
{
    private readonly MyAccountService _myAccountService;
    private readonly AuthSdkMapperService _authSdkMapper;
    private readonly ISessionStorage _sessionStorage;

    public MyAccountController(MyAccountService myAccountService, AuthSdkMapperService authSdkMapper, ISessionStorage sessionStorage)
    {
        _myAccountService = myAccountService;
        _authSdkMapper = authSdkMapper;
        _sessionStorage = sessionStorage;
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
    [ProducesResponseType(typeof(ClaimsPrincipal), StatusCodes.Status200OK)]
    [Authorize]
    public IActionResult MyProfileGetActionAsync()
    {
        var identity = _sessionStorage.Identity;
        var claims = IdentityMapper.ToClaimsPrincipal(identity);

        var claimsDictionary = claims.Claims.GroupBy(c => c.Type)
            .ToDictionary(g => g.Key, g => g.Select(c => c.Value).ToArray());
        return Ok(claimsDictionary);
    }

    /// <summary>
    ///     Get my sessions.
    /// </summary>
    [HttpGet("sessions")]
    [ProducesResponseType(typeof(MyAccountSessionInfo[]), StatusCodes.Status200OK)]
    public async Task<IActionResult> MySessionsGetActionAsync()
    {
        var response = await _myAccountService.FindMySessions();

        if (response.Status != ResponseStatus.Ok)
            return response.ToErrorActionResult();

        var infos = new List<MyAccountSessionInfo>();
        foreach (var item in response.Payload!)
            infos.Add(await _authSdkMapper.ToSessionInfoAsync(item));

        return Ok(infos);
    }

    /// <summary>
    ///     Revoke session.
    /// </summary>
    [HttpDelete("sessions/{sessionId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [Authorize]
    public async Task<IActionResult> RevokeSessionActionAsync(Guid sessionId)
    {
        var response = await _myAccountService.RevokeAsync(sessionId);

        if (response.Status == ResponseStatus.Fail)
            return response.ToErrorActionResult();

        return NoContent();
    }

    /// <summary>
    ///     Application info for consent.
    /// </summary>
    [HttpGet("application-consent/{clientId}")]
    [ProducesResponseType(typeof(AuthClient), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [Authorize]
    public async Task<IActionResult> ClientInfoActionAsync(string clientId)
    {
        var response = await _myAccountService.ApplicationConsentInfoGetAsync(clientId);

        if (response.Status == ResponseStatus.Fail)
            return response.ToErrorActionResult();

        return Ok(_authSdkMapper.ToClient(response.Payload!));
    }

    /// <summary>
    ///     Application info for consent.
    /// </summary>
    [HttpPost("application-consent/{clientId}")]
    [ProducesResponseType(typeof(TokenInfo), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [Authorize]
    public async Task<IActionResult> ClientInfoConsentActionAsync(string clientId,
        [FromQuery(Name = "redirect_uri")] string redirectUri)
    {
        var ipAddress = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var userAgentString = Request.Headers.UserAgent;
        UserAgentInfo? userAgent = null;
        if (!string.IsNullOrWhiteSpace(userAgentString))
            userAgent = UserAgentInfoParser.Parse(userAgentString!);

        var response = await _myAccountService.ApplicationConsentAsync(clientId, redirectUri, ipAddress, userAgent);

        if (response.Status == ResponseStatus.Fail)
            return Unauthorized();

        var json =
            $@"{{""access_token"": ""{response.Payload?.AccessToken}"", ""expires_in"": ""{response.Payload?.ExpiresIn}""}}";

        return Ok(json);
    }
}