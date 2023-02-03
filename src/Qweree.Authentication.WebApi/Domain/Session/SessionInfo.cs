using System;
using Qweree.Authentication.WebApi.Domain.Authentication;

namespace Qweree.Authentication.WebApi.Domain.Session;

public class SessionInfo
{
    public SessionInfo(Guid id, Guid clientId, Guid? userId, string refreshToken, string ipAddress, UserAgentInfo? userAgent, GrantType grant,
        DateTime createdAt, DateTime issuedAt, DateTime expiresAt)
    {
        Id = id;
        ClientId = clientId;
        UserId = userId;
        RefreshToken = refreshToken;
        IpAddress = ipAddress;
        UserAgent = userAgent;
        Grant = grant;
        CreatedAt = createdAt;
        IssuedAt = issuedAt;
        ExpiresAt = expiresAt;
    }

    public Guid Id { get; }
    public Guid ClientId { get; }
    public Guid? UserId { get; }
    public string RefreshToken { get; }
    public string IpAddress { get; }
    public UserAgentInfo? UserAgent { get; }
    public GrantType Grant { get; }
    public DateTime CreatedAt { get; }
    public DateTime IssuedAt { get; }
    public DateTime ExpiresAt { get; }
}