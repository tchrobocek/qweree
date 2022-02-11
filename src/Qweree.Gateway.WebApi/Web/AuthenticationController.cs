using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Qweree.Authentication.Sdk.OAuth2;
using Qweree.Authentication.Sdk.Tokens;
using Qweree.Gateway.Sdk;
using Qweree.Gateway.WebApi.Infrastructure;
using Qweree.Gateway.WebApi.Infrastructure.Session;
using Qweree.Utils;

namespace Qweree.Gateway.WebApi.Web;

[ApiController]
[Route("/api/v1/auth")]
public class AuthenticationController : ControllerBase
{
    private readonly Random _random = new();
    private readonly OAuth2Client _oauthClient;
    private readonly ISessionStorage _sessionStorage;
    private readonly IWebHostEnvironment _environment;
    private readonly IOptions<QwereeConfigurationDo> _qwereeConfig;

    public AuthenticationController(OAuth2Client oauthClient, ISessionStorage sessionStorage,
        IWebHostEnvironment environment, IOptions<QwereeConfigurationDo> qwereeConfig)
    {
        _oauthClient = oauthClient;
        _sessionStorage = sessionStorage;
        _environment = environment;
        _qwereeConfig = qwereeConfig;
    }

    /// <summary>
    ///     Login.
    /// </summary>
    /// <returns>Identity.</returns>
    [HttpPost]
    [Route("login")]
    [ProducesResponseType(typeof(IdentityDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> LoginAsync(LoginInputDto input)
    {
        var grantInput = new PasswordGrantInput(input.Username ?? string.Empty, input.Password ?? string.Empty);
        var clientCredentials = new ClientCredentials(_qwereeConfig.Value.ClientId ?? string.Empty, _qwereeConfig.Value.ClientSecret ?? string.Empty);
        var response = await _oauthClient.SignInAsync(grantInput, clientCredentials);

        if (!response.IsSuccessful)
        {
            return BadRequest(await response.ReadErrorsAsync());
        }

        var tokenInfo = (await response.ReadPayloadAsync(JsonUtils.SnakeCaseNamingPolicy))!;
        var cookie = GenerateCookie();

        Response.Cookies.Delete("Session");
        if (_environment.IsDevelopment())
        {
            Response.Cookies.Append("Session", cookie, new CookieOptions
            {
                HttpOnly = false,
                SameSite = SameSiteMode.Strict,
                Secure = true
            });
        }

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

        return Ok(IdentityMapper.FromClaimsPrincipal(new ClaimsPrincipal(user)));
    }

    /// <summary>
    ///     Logout, removes cookie and all the leftovers.
    /// </summary>
    [HttpPost]
    [Route("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> LogoutAsync()
    {
        var cookie = Request.Cookies["Session"];
        if (cookie == null)
            return NoContent();

        Response.Cookies.Delete("Session");
        await _sessionStorage.DeleteAsync(cookie);
        return NoContent();
    }

    /// <summary>
    ///     Refresh access token.
    /// </summary>
    [HttpPost]
    [Route("refresh")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RefreshAsync()
    {
        var cookie = Request.Cookies["Session"];
        if (cookie == null)
            return Unauthorized();

        TokenInfoDto? tokenInfo;

        try
        {
            await using var stream = await _sessionStorage.ReadAsync(cookie);
            tokenInfo = await JsonUtils.DeserializeAsync<TokenInfoDto>(stream);
        }
        catch (ArgumentException)
        {
            Response.Cookies.Delete("Session");
            return Unauthorized();
        }
        var response = await _oauthClient.RefreshAsync(new RefreshTokenGrantInput(tokenInfo!.RefreshToken ?? string.Empty),
            new ClientCredentials(_qwereeConfig.Value.ClientId ?? string.Empty,
                _qwereeConfig.Value.ClientSecret ?? string.Empty));

        if (!response.IsSuccessful)
            return StatusCode((int)response.StatusCode, await response.ReadErrorsAsync());

        var dto = await response.ReadPayloadAsync(JsonUtils.SnakeCaseNamingPolicy);
        await using var memoryStream = new MemoryStream();
        await JsonUtils.SerializeAsync(memoryStream, dto!);
        memoryStream.Seek(0, SeekOrigin.Begin);
        await _sessionStorage.WriteAsync(cookie, memoryStream);
        return NoContent();
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