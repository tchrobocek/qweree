using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Qweree.Authentication.Sdk.OAuth2
{
    public class UserMapper
    {
        public static ClaimsPrincipal ToClaimsPrincipal(UserDto userDto)
        {
            var claims = new List<Claim>();
            if (userDto.Id != null)
                claims.Add(new Claim("user_id", userDto.Id?.ToString() ?? string.Empty));
            if (userDto.Username != null)
                claims.Add(new Claim("username", userDto.Username));
            if (userDto.FullName != null)
                claims.Add(new Claim("full_name", userDto.FullName));
            if (userDto.Email != null)
                claims.Add(new Claim("full_name", userDto.Email));
            if (userDto.Roles != null)
                claims.AddRange(userDto.Roles.Select(r => new Claim("role", r)));

            return new ClaimsPrincipal(new ClaimsIdentity(claims));
        }

        public static UserDto FromClaimsPrincipal(ClaimsPrincipal claimsPrincipal)
        {
            if (!claimsPrincipal.Identity?.IsAuthenticated ?? false)
            {
                throw new ArgumentException("User is not authenticated.");
            }

            var id = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
            var username = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "username")?.Value;
            var fullName = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "full_name")?.Value;
            var email = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
            var roles = claimsPrincipal.Claims.Where(c => c.Type == "role").Select(c => c.Value);

            return new UserDto
            {
                Id = Guid.Parse(id ?? Guid.Empty.ToString()),
                Email = email,
                Username = username,
                FullName = fullName,
                Roles = roles.ToArray()
            };
        }
    }
}