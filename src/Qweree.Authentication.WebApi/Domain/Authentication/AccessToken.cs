using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Qweree.Authentication.WebApi.Domain.Authentication
{
    public class AccessToken
    {
        public AccessToken(string username, IEnumerable<string> roles, DateTime issuedAt, DateTime expiresAt)
        {
            Username = username;
            IssuedAt = issuedAt;
            ExpiresAt = expiresAt;
            Roles = roles.ToImmutableArray();
        }

        public ImmutableArray<string> Roles { get; }
        public string Username { get; }
        public DateTime IssuedAt { get; }
        public DateTime ExpiresAt { get; }
    }
}