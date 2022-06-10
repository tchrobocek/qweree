using System;

namespace Qweree.Authentication.Sdk.Session.Tokens;

public class AccessToken
{
    public AccessToken(Guid sessionId, Identity identity, DateTime issuedAt, DateTime expiresAt)
    {
        SessionId = sessionId;
        Identity = identity;
        IssuedAt = issuedAt;
        ExpiresAt = expiresAt;
    }

    public Guid SessionId { get; }
    public Identity Identity { get; }
    public DateTime IssuedAt { get; }
    public DateTime ExpiresAt { get; }
}