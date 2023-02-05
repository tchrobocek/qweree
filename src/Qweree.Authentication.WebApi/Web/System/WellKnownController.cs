using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Qweree.Authentication.Sdk.OpenId;
using Qweree.Authentication.WebApi.Infrastructure;

namespace Qweree.Authentication.WebApi.Web.System;

[ApiController]
[Route(".well-known")]
public class WellKnownController : ControllerBase
{
    private readonly RSA _rsa;
    private readonly IOptions<QwereeConfigurationDo> _qwereeOptions;

    public WellKnownController(IOptions<QwereeConfigurationDo> qwereeOptions, RSA rsa)
    {
        _qwereeOptions = qwereeOptions;
        _rsa = rsa;
    }

    /// <summary>
    ///     Get openid configuration.
    /// </summary>
    /// <returns>Returns current project assembly version.</returns>
    [HttpGet("openid-configuration")]
    [ProducesResponseType(typeof(OpenIdConfiguration), StatusCodes.Status200OK)]
    public IActionResult OpenIdConfigurationGetAction()
    {
        var issuer = new Uri($"{Request.Scheme}://{Request.Host}");
        var baseUri = new Uri(issuer, _qwereeOptions.Value.PathBase + "/");

        return Ok(new OpenIdConfiguration
        {
            Issuer = issuer.ToString(),
            TokenEndpoint = new Uri(baseUri, "api/oauth2/auth").ToString(),
            JwksUri = new Uri(baseUri, ".well-known/jwks.json").ToString(),
            GrantTypesSupported = new[] {"client_credentials", "password", "refresh_token", "implicit"},
            ResponseTypesSupported = new[] {"token", "token id_token"},
            AuthorizationEndpoint = new Uri(baseUri, "/authorize").ToString(),
            UserInfoEndpoint = null,
            RegistrationEndpoint = null
        });
    }

    /// <summary>
    ///     Get JWKs.
    /// </summary>
    /// <returns>Returns current project assembly version.</returns>
    [HttpGet("jwks.json")]
    [ProducesResponseType(typeof(JwksSet), StatusCodes.Status200OK)]
    public IActionResult JwksGetAction()
    {
        var publicKey = _rsa.ExportSubjectPublicKeyInfo();
        var parameters = _rsa.ExportParameters(false);
        return Ok(new JwksSet
        {
            Keys = new[]
            {
                new Jwk
                {
                    KeyType = "RSA",
                    Use = "sig",
                    Modulus = Convert.ToBase64String(parameters.Modulus!),
                    Exponent = Convert.ToBase64String(parameters.Exponent!),
                    CertificateChain = new[] { Convert.ToBase64String(publicKey) }
                }
            }
        });
    }
}