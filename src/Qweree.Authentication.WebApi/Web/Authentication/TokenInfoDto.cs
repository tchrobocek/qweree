using System;

namespace Qweree.Authentication.WebApi.Web.Authentication
{
    public class TokenInfoDto
    {
        public string? AccessToken { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }
}