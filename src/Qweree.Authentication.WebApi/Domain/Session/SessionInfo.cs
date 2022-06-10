using System;
using Qweree.Authentication.WebApi.Domain.Authentication;

namespace Qweree.Authentication.WebApi.Domain.Session;

public class SessionInfo
{
    public SessionInfo(Guid id, Guid clientId, Guid? userId, string refreshToken, DeviceInfo? device, GrantType grant,
        DateTime createdAt, DateTime issuedAt, DateTime expiresAt)
    {
        Id = id;
        ClientId = clientId;
        UserId = userId;
        RefreshToken = refreshToken;
        Device = device;
        Grant = grant;
        CreatedAt = createdAt;
        IssuedAt = issuedAt;
        ExpiresAt = expiresAt;
    }

    public Guid Id { get; }
    public Guid ClientId { get; }
    public Guid? UserId { get; }
    public string RefreshToken { get; }
    public DeviceInfo? Device { get; }
    public GrantType Grant { get; }
    public DateTime CreatedAt { get; }
    public DateTime IssuedAt { get; }
    public DateTime ExpiresAt { get; }
}