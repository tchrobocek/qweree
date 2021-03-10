using System;

namespace Qweree.Authentication.Sdk.Tokens
{
    public class TokenInfo
    {
        public TokenInfo(string accessToken, DateTime expiresAt) : this(accessToken, null, expiresAt)
        {
        }

        public TokenInfo(string accessToken, string? refreshToken, DateTime expiresAt)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
            ExpiresAt = expiresAt;
        }

        public string AccessToken { get; }
        public string? RefreshToken { get; }
        public DateTime ExpiresAt { get; }
    }
}