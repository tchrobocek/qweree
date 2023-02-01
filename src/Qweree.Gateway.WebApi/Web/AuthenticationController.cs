using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Qweree.Authentication.Sdk.OAuth2;
using Qweree.Authentication.Sdk.Session;
using Qweree.Gateway.Sdk;
using Qweree.Gateway.WebApi.Infrastructure;
using Qweree.Utils;
using ISessionStorage = Qweree.Gateway.WebApi.Infrastructure.Session.ISessionStorage;

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
    private readonly HttpMessageHandler _messageHandler;

    public AuthenticationController(OAuth2Client oauthClient, ISessionStorage sessionStorage,
        IWebHostEnvironment environment, IOptions<QwereeConfigurationDo> qwereeConfig,
        HttpMessageHandler messageHandler)
    {
        _oauthClient = oauthClient;
        _sessionStorage = sessionStorage;
        _environment = environment;
        _qwereeConfig = qwereeConfig;
        _messageHandler = messageHandler;
    }

    /// <summary>
    ///     Login.
    /// </summary>
    /// <returns>Identity.</returns>
    [HttpPost]
    [Route("login")]
    [ProducesResponseType(typeof(Identity), StatusCodes.Status200OK)]
    public async Task<IActionResult> LoginAsync(LoginInputDto input)
    {
        var grantInput = new PasswordGrantInput
        {
            Username = input.Username,
            Password = input.Password
        };
        var clientCredentials = new ClientCredentials
        {
            ClientId = _qwereeConfig.Value.ClientId,
            ClientSecret = _qwereeConfig.Value.ClientSecret
        };
        var options = new OAuth2RequestOptions(Request.Headers.UserAgent);
        var response = await _oauthClient.SignInAsync(grantInput, clientCredentials, options);

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

        return Ok(IdentityMapper.ToIdentity(new ClaimsPrincipal(user)));
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

        try
        {
            var client = await CreateAuthenticatedClientAsync();
            var response = await client.RevokeAsync();

            if (!response.IsSuccessful)
                return StatusCode((int)response.StatusCode, await response.ReadErrorsAsync());

            await _sessionStorage.DeleteAsync(cookie);
        }
        catch (Exception)
        {
            // ignored
        }
        finally
        {
            Response.Cookies.Delete("Session");
        }

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

        TokenInfo? tokenInfo;

        try
        {
            await using var stream = await _sessionStorage.ReadAsync(cookie);
            tokenInfo = await JsonUtils.DeserializeAsync<TokenInfo>(stream);
        }
        catch (ArgumentException)
        {
            Response.Cookies.Delete("Session");
            return Unauthorized();
        }

        var options = new OAuth2RequestOptions(Request.Headers.UserAgent);
        var response = await _oauthClient.RefreshAsync(new RefreshTokenGrantInput
            {
                RefreshToken = tokenInfo?.RefreshToken
            },
            new ClientCredentials
            {
                ClientId = _qwereeConfig.Value.ClientId,
                ClientSecret = _qwereeConfig.Value.ClientSecret
            }, options);

        if (!response.IsSuccessful)
            return StatusCode((int)response.StatusCode, await response.ReadErrorsAsync());

        var dto = await response.ReadPayloadAsync(JsonUtils.SnakeCaseNamingPolicy);
        await using var memoryStream = new MemoryStream();
        await JsonUtils.SerializeAsync(memoryStream, dto!);
        memoryStream.Seek(0, SeekOrigin.Begin);
        await _sessionStorage.WriteAsync(cookie, memoryStream);
        return NoContent();
    }

    private async Task<OAuth2Client> CreateAuthenticatedClientAsync(CancellationToken cancellationToken = new())
    {
        var cookie = Request.Cookies["Session"];

        TokenInfo? tokenInfo = null;
        if (cookie != null)
        {
            await using var stream = await _sessionStorage.ReadAsync(cookie, cancellationToken);
            tokenInfo = await JsonUtils.DeserializeAsync<TokenInfo>(stream, cancellationToken);
        }

        var client = new HttpClient(_messageHandler)
        {
            BaseAddress = new Uri(new Uri(_qwereeConfig.Value.AuthUri!), "api/oauth2/"),
        };
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenInfo?.AccessToken);
        return new OAuth2Client(client);
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