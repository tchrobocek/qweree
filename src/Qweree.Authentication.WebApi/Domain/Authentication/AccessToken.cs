using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Qweree.Authentication.WebApi.Domain.Authentication
{
    public class AccessToken
    {
        public AccessToken(string clientId, string email, IEnumerable<string> roles, DateTime issuedAt, DateTime expiresAt)
        {
            ClientId = clientId;
            Email = email;
            Roles = roles.ToImmutableArray();
            IssuedAt = issuedAt;
            ExpiresAt = expiresAt;
        }
        public AccessToken(string clientId, Guid userId, string username, string fullName, string email, IEnumerable<string> roles,
            DateTime issuedAt, DateTime expiresAt)
        {
            UserId = userId;
            Username = username;
            IssuedAt = issuedAt;
            ExpiresAt = expiresAt;
            ClientId = clientId;
            FullName = fullName;
            Email = email;
            Roles = roles.ToImmutableArray();
        }

        public string ClientId { get; }
        public Guid? UserId { get; }
        public string? Username { get; }
        public string? FullName { get; }
        public string Email { get; }
        public DateTime IssuedAt { get; }
        public DateTime ExpiresAt { get; }
        public ImmutableArray<string>? Roles { get; }
    }
}