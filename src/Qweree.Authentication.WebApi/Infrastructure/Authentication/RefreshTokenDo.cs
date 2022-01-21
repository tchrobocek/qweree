using System;

namespace Qweree.Authentication.WebApi.Infrastructure.Authentication;

public class RefreshTokenDo
{
    public Guid Id { get; set; }
    public string? Token { get; set; }
    public Guid UserId { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid ClientId { get; set; }
}