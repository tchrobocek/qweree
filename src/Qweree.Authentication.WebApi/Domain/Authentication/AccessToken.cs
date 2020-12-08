using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Qweree.Authentication.WebApi.Domain.Authentication
{
    public class AccessToken
    {
        public AccessToken(Guid userId, string username, IEnumerable<string> roles, DateTime issuedAt, DateTime expiresAt)
        {
            UserId = userId;
            Username = username;
            IssuedAt = issuedAt;
            ExpiresAt = expiresAt;
            Roles = roles.ToImmutableArray();
        }

        public Guid UserId { get; }
        public string Username { get; }
        public DateTime IssuedAt { get; }
        public DateTime ExpiresAt { get; }
        public ImmutableArray<string> Roles { get; }
    }
}