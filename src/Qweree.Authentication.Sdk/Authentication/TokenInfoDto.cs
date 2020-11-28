using System;

namespace Qweree.Authentication.Sdk.Authentication
{
    public class TokenInfoDto
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }
}