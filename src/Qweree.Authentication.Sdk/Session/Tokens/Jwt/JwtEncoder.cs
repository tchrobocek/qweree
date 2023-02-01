using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Qweree.Authentication.Sdk.Session.Tokens.Jwt;

public class JwtEncoder : ITokenEncoder
{
    private readonly string _issuer;
    private readonly string _audience;
    private readonly string _accessTokenKey;

    public static AccessToken Decode(string accessToken)
    {
        var token = (JwtSecurityToken)new JwtSecurityTokenHandler()
            .ReadToken(accessToken);
        var claims = token.Claims.ToList();
        claims.AddRange(token.Claims.Where(c => c.Type == "role")
            .Select(c => new Claim(ClaimTypes.Role, c.Value)).ToArray());

        var expClaim = claims.FirstOrDefault(c => c.Type == "exp");
        var exp = expClaim?.Value ?? "0";
        var expTime = DateTime.UnixEpoch;
        expTime = expTime.AddSeconds(int.Parse(exp));

        var sidClaim = claims.FirstOrDefault(c => c.Type == "sid");
        if (!Guid.TryParse(sidClaim?.Value ?? String.Empty, out var sessionId))
            sessionId = Guid.Empty;

        var identity = IdentityMapper.ToIdentity(claims);
        return new AccessToken
        {
            SessionId = sessionId,
            Identity = identity,
            IssuedAt = token.IssuedAt,
            ExpiresAt = expTime
        };
    }

    public JwtEncoder(string issuer, string audience, string accessTokenKey)
    {
        _issuer = issuer;
        _audience = audience;
        _accessTokenKey = accessTokenKey;
    }

    public string EncodeAccessToken(AccessToken accessToken)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_accessTokenKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var identityPrincipal = FromDto(accessToken.Identity!);
        var claims = new List<Claim>(identityPrincipal.Claims)
        {
            new("iat", new DateTimeOffset(accessToken.IssuedAt ?? DateTime.MinValue).ToUnixTimeSeconds().ToString()),
            new("jti", Guid.NewGuid().ToString()),
            new("sid", accessToken.SessionId?.ToString() ?? Guid.Empty.ToString())
        };

        var token = new JwtSecurityToken(_issuer, _audience, claims,
            expires: accessToken.ExpiresAt, signingCredentials: credentials,
            notBefore: accessToken.IssuedAt);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public AccessToken DecodeAccessToken(string accessToken)
    {
        return Decode(accessToken);
    }
    public static ClaimsPrincipal FromDto(Identity identity)
    {
        var claims = new List<Claim>
        {
            new("client.id", identity.Client?.Id.ToString() ?? Guid.Empty.ToString()),
            new("client.client_id", identity.Client?.ClientId ?? string.Empty),
            new("client.application_name", identity.Client?.ApplicationName ?? string.Empty),
            new("email", identity.Email ?? string.Empty)
        };

        claims.AddRange(identity.Roles?.Select(r => new Claim("role", r)) ?? Array.Empty<Claim>());
        claims.AddRange(identity.Roles?.Select(r => new Claim(ClaimTypes.Role, r)) ?? Array.Empty<Claim>());

        if (identity.User != null)
        {
            claims.Add(new Claim("user.id", identity.User?.Id?.ToString() ?? string.Empty));
            claims.Add(new Claim("user.username", identity.User?.Username ?? string.Empty));
            claims.AddRange(identity.User?.Properties?.Select(prop => new Claim($"user.property.{prop.Key}", prop.Value ?? string.Empty)) ?? Array.Empty<Claim>());
        }

        return new ClaimsPrincipal(new ClaimsIdentity(claims, "qweree"));
    }
}