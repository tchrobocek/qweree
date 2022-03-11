using System;

namespace Qweree.Authentication.WebApi.Domain.Authentication;

public class RefreshToken
{
    public RefreshToken(Guid id, string token, Guid clientId, Guid userId, DateTime expiresAt, DateTime createdAt, DeviceInfo? device)
    {
        Id = id;
        Token = token;
        ClientId = clientId;
        UserId = userId;
        ExpiresAt = expiresAt;
        CreatedAt = createdAt;
        Device = device;
    }

    public Guid Id { get; }
    public string Token { get; }
    public Guid ClientId { get; }
    public Guid UserId { get; }
    public DateTime ExpiresAt { get; }
    public DateTime CreatedAt { get; }
    public DeviceInfo? Device { get; }
}