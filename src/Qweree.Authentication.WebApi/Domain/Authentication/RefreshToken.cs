using System;

namespace Qweree.Authentication.WebApi.Domain.Authentication
{
    public class RefreshToken
    {
        public RefreshToken(Guid id, string token, Guid userId, DateTime expiresAt, DateTime createdAt)
        {
            Id = id;
            Token = token;
            UserId = userId;
            ExpiresAt = expiresAt;
            CreatedAt = createdAt;
        }

        public Guid Id { get; }
        public string Token { get; }
        public Guid UserId { get; }
        public DateTime ExpiresAt { get; }
        public DateTime CreatedAt { get; }
    }
}