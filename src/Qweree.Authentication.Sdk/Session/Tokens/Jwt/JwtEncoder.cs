using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace Qweree.Authentication.Sdk.Session.Tokens.Jwt;

public class JwtEncoder : ITokenEncoder
{
    private readonly string _issuer;

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

    public JwtEncoder(string issuer)
    {
        _issuer = issuer;
    }

    public string EncodeAccessToken(AccessToken accessToken, RsaSecurityKey rsa)
    {
        var credentials = new SigningCredentials(rsa, SecurityAlgorithms.RsaSha256Signature);

        var identityPrincipal = IdentityMapper.ToClaimsPrincipal(accessToken.Identity!);
        var claims = new List<Claim>(identityPrincipal.Claims)
        {
            new("iat", new DateTimeOffset(accessToken.IssuedAt ?? DateTime.MinValue).ToUnixTimeSeconds().ToString()),
            new("jti", Guid.NewGuid().ToString()),
            new("sid", accessToken.SessionId?.ToString() ?? Guid.Empty.ToString())
        };

        var token = new JwtSecurityToken(_issuer, null, claims,
            expires: accessToken.ExpiresAt, signingCredentials: credentials,
            notBefore: accessToken.IssuedAt);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public AccessToken DecodeAccessToken(string accessToken)
    {
        return Decode(accessToken);
    }
}