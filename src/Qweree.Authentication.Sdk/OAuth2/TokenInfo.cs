using System;

namespace Qweree.Authentication.Sdk.OAuth2;

public class TokenInfo
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? ExpiresAt { get; set; }
}