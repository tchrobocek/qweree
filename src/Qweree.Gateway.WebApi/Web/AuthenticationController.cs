using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Qweree.Authentication.Sdk.OAuth2;
using Qweree.Gateway.Sdk;
using Qweree.Gateway.WebApi.Infrastructure.Session;
using Qweree.Utils;

namespace Qweree.Gateway.WebApi.Web;

[ApiController]
[Route("/api/v1/auth")]
public class AuthenticationController : ControllerBase
{
    private readonly Random _random = new();
    private readonly OAuth2Client _oauthClient;
    private readonly SessionStorage _sessionStorage;

    public AuthenticationController(OAuth2Client oauthClient, SessionStorage sessionStorage)
    {
        _oauthClient = oauthClient;
        _sessionStorage = sessionStorage;
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> LoginAsync(LoginInputDto input)
    {
        var grantInput = new PasswordGrantInput(input.Username ?? string.Empty, input.Password ?? string.Empty);
        var clientCredentials = new ClientCredentials("admin-cli", "password");
        var response = await _oauthClient.SignInAsync(grantInput, clientCredentials);

        if (!response.IsSuccessful)
        {
            return BadRequest(await response.ReadErrorsAsync());
        }

        var tokenInfo = (await response.ReadPayloadAsync(JsonUtils.SnakeCaseNamingPolicy))!;
        var cookie = GenerateCookie();
        Response.Cookies.Append("Session", cookie, new CookieOptions
        {
            Expires = new DateTimeOffset(DateTime.UtcNow + TimeSpan.FromHours(3)),
            MaxAge = TimeSpan.FromDays(1),
            HttpOnly = false,
            SameSite = SameSiteMode.None,
            Secure = true
        });

        await using var memoryStream = new MemoryStream();
        await memoryStream.WriteAsync(Encoding.UTF8.GetBytes(JsonUtils.Serialize(tokenInfo)));
        memoryStream.Seek(0, SeekOrigin.Begin);

        await _sessionStorage.WriteAsync(cookie, memoryStream);

        var accessToken = tokenInfo.AccessToken!;

        ClaimsIdentity user;

        if (string.IsNullOrWhiteSpace(accessToken))
        {
            user = new ClaimsIdentity(authenticationType: null);
        }
        else
        {
            try
            {
                var token = (JwtSecurityToken)new JwtSecurityTokenHandler()
                    .ReadToken(accessToken);
                var claims = token.Claims.ToList();
                claims.AddRange(token.Claims.Where(c => c.Type == "role")
                    .Select(c => new Claim(ClaimTypes.Role, c.Value)).ToArray());
                user = new ClaimsIdentity(claims, "oauth2");
            }
            catch (Exception)
            {
                user = new ClaimsIdentity(authenticationType: null);
            }
        }

        return Ok(UserMapper.FromClaimsPrincipal(new ClaimsPrincipal(user)));
    }

    private string GenerateCookie()
    {
        const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        const int secretLength = 30;
        var result = string.Empty;

        for (var i = 0; i < secretLength; i++)
        {
            var index = _random.Next(chars.Length);
            result += chars[index];
        }

        return result;
    }
}