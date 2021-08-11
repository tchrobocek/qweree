using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Qweree.WebApplication.Infrastructure.Authentication
{
    public static class TokenDecoder
    {
        public static IEnumerable<Claim> ReadClaims(string jwt)
        {
            var token = (JwtSecurityToken)new JwtSecurityTokenHandler().ReadToken(jwt);
            return token.Claims;
        }
    }
}