using System;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using DeviceDetectorNET;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Qweree.AspNet.Application;
using Qweree.Authentication.Sdk.OAuth2;
using Qweree.Authentication.WebApi.Domain.Authentication;
using Qweree.Authentication.WebApi.Infrastructure.Security;
using Qweree.Utils;
using ClientCredentials = Qweree.Authentication.WebApi.Domain.Authentication.ClientCredentials;
using PasswordGrantInput = Qweree.Authentication.WebApi.Domain.Authentication.PasswordGrantInput;
using RefreshTokenGrantInput = Qweree.Authentication.WebApi.Domain.Authentication.RefreshTokenGrantInput;

namespace Qweree.Authentication.WebApi.Web.OAuth2;

[ApiController]
[Route("/api/oauth2")]
public class OAuth2Controller : ControllerBase
{
    private static readonly ImmutableArray<string> GrantWhitelist =
        new[] { "client_credentials", "password", "refresh_token" }.ToImmutableArray();

    private readonly AuthorizationHeaderEncoder _authorizationHeaderEncoder = new();
    private readonly AuthenticationService _authenticationService;
    private readonly IDateTimeProvider _datetimeProvider;

    public OAuth2Controller(AuthenticationService authenticationService, IDateTimeProvider datetimeProvider)
    {
        _authenticationService = authenticationService;
        _datetimeProvider = datetimeProvider;
    }

    /// <summary>
    ///     Authenticate user.
    /// </summary>
    /// <param name="accessToken">Access token.</param>
    /// <param name="clientSecret">Client secret</param>
    /// <param name="grantType">Grant type, either ["password", "refresh_token"].</param>
    /// <param name="username">Username.</param>
    /// <param name="password">Password.</param>
    /// <param name="refreshToken">Refresh token.</param>
    /// <param name="clientId">Client id.</param>
    /// <param name="authorizationHeader">Authorization header.</param>
    /// <returns>Created user.</returns>
    [HttpPost("auth")]
    [ProducesResponseType(typeof(TokenInfoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AuthenticateActionAsync(
        [FromForm(Name = "username")] string? username,
        [FromForm(Name = "password")] string? password,
        [FromForm(Name = "refresh_token")] string? refreshToken,
        [FromForm(Name = "access_token")] string? accessToken,
        [FromForm(Name = "client_id")] string? clientId,
        [FromForm(Name = "client_secret")] string? clientSecret,
        [Required] [FromForm(Name = "grant_type")] string grantType,
        [FromHeader(Name = "Authorization")] string? authorizationHeader)
    {
        if (!GrantWhitelist.Contains(grantType))
            return Unauthorized();

        Response<TokenInfo> response;

        var clientCredentials = new ClientCredentials(clientId ?? "", clientSecret);

        if (!string.IsNullOrWhiteSpace(authorizationHeader))
        {
            if (authorizationHeader.StartsWith("Basic "))
            {
                var hash = authorizationHeader.Substring("Basic ".Length).Trim();

                try
                {
                    clientCredentials = _authorizationHeaderEncoder.Decode(hash);
                }
                catch (Exception)
                {
                    return Unauthorized();
                }
            }
        }

        var device = GetDeviceInfo();
        if (grantType == "password")
        {
            var passwordInput = new PasswordGrantInput(username ?? "", password ?? "");
            response = await _authenticationService.AuthenticateAsync(passwordInput, clientCredentials, device);
        }
        else if (grantType == "refresh_token")
        {
            var refreshTokenInput = new RefreshTokenGrantInput(refreshToken ?? "");
            response = await _authenticationService.AuthenticateAsync(refreshTokenInput, clientCredentials, device);
        }
        else if (grantType == "client_credentials")
        {
            response = await _authenticationService.AuthenticateAsync(clientCredentials, device);
        }
        else
        {
            return Unauthorized();
        }

        if (response.Status == ResponseStatus.Fail)
            return Unauthorized();

        var expiresIn = response.Payload?.ExpiresAt - _datetimeProvider.UtcNow;

        var refreshTokenJson = "";
        if (response.Payload?.RefreshToken != null)
            refreshTokenJson = @$", ""refresh_token"": ""{response.Payload.RefreshToken}""";

        var json =
            $@"{{""access_token"": ""{response.Payload?.AccessToken}""{refreshTokenJson}, ""expires_in"": ""{expiresIn?.TotalSeconds}""}}";

        return Ok(json);
    }

    /// <summary>
    ///     Revoke current session.
    /// </summary>
    [HttpPost("revoke")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Authorize]
    public async Task<IActionResult> RevokeSessionActionAsync()
    {
        var response = await _authenticationService.RevokeAsync();

        if (response.Status == ResponseStatus.Fail)
            return Unauthorized();

        return NoContent();
    }

    private DeviceInfo? GetDeviceInfo()
    {
        var userAgent = Request.Headers.UserAgent;
        if (string.IsNullOrWhiteSpace(userAgent))
            return null;

        var detector = new DeviceDetector(userAgent);
        detector.Parse();

        if(detector.IsBot())
        {
            var result = detector.GetBot();

            if (result.Success)
                return DeviceInfo.Bot(result.ToString());

            return null;
        }
        else
        {
            var clientInfo = detector.GetClient();
            var osInfo = detector.GetOs();

            var client = string.Empty;
            var os = string.Empty;

            if (clientInfo.Success)
                client = clientInfo.ToString();
            if (osInfo.Success)
                os = osInfo.ToString();
            var device = detector.GetDeviceName();
            var brand  = detector.GetBrandName();
            var model  = detector.GetModel();

            return new DeviceInfo(client, os, device, brand, model);
        }
    }
}