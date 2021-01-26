using System;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Qweree.AspNet.Application;
using Qweree.Authentication.Sdk.Authentication;
using Qweree.Authentication.WebApi.Domain.Authentication;
using Qweree.Utils;
using PasswordGrantInput = Qweree.Authentication.WebApi.Domain.Authentication.PasswordGrantInput;
using RefreshTokenGrantInput = Qweree.Authentication.WebApi.Domain.Authentication.RefreshTokenGrantInput;

namespace Qweree.Authentication.WebApi.Web.Authentication
{
    [ApiController]
    [Route("/api/oauth2/auth")]
    public class OAuth2Controller : ControllerBase
    {
        private static readonly ImmutableArray<string> GrantWhitelist =
            new[] {"password", "refresh_token", "file_access"}.ToImmutableArray();

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
        /// <param name="grantType">Grant type, either ["password", "refresh_token"].</param>
        /// <param name="username">Username.</param>
        /// <param name="password">Password.</param>
        /// <param name="refreshToken">Refresh token.</param>
        /// <returns>Created user.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(TokenInfoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AuthenticateActionAsync(
            [FromForm(Name = "username")] string? username,
            [FromForm(Name = "password")] string? password,
            [FromForm(Name = "refresh_token")] string? refreshToken,
            [FromForm(Name = "access_token")] string? accessToken,
            [Required] [FromForm(Name = "grant_type")]
            string grantType)
        {
            if (!GrantWhitelist.Contains(grantType))
                return Unauthorized();

            Response<TokenInfo> response;

            if (grantType == "password")
            {
                var passwordInput = new PasswordGrantInput(username ?? "", password ?? "");
                response = await _authenticationService.AuthenticateAsync(passwordInput);
            }
            else if (grantType == "refresh_token")
            {
                var refreshTokenInput = new RefreshTokenGrantInput(refreshToken ?? "");
                response = await _authenticationService.AuthenticateAsync(refreshTokenInput);
            }
            else if (grantType == "file_access")
            {
                FileAccessGrantInput? fileAccessInput = null;

                if (User.Identity?.IsAuthenticated ?? false)
                    fileAccessInput =
                        new FileAccessGrantInput(Request.Headers[HeaderNames.Authorization].First() ?? "");

                response = await _authenticationService.AuthenticateAsync(fileAccessInput ??
                                                                          new FileAccessGrantInput(accessToken ?? ""));
            }
            else
            {
                throw new InvalidOperationException("Invalid grant.");
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
    }
}