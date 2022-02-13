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

        var identity = ClaimsPrincipalMapper.CreateIdentity(claims);
        return new AccessToken(identity, token.IssuedAt, expTime);
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

        var identityPrincipal = ClaimsPrincipalMapper.CreateClaimsPrincipal(accessToken.Identity);
        var claims = new List<Claim>(identityPrincipal.Claims)
        {
            new("iat", new DateTimeOffset(accessToken.IssuedAt).ToUnixTimeSeconds().ToString()),
            new("jti", Guid.NewGuid().ToString())
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
}