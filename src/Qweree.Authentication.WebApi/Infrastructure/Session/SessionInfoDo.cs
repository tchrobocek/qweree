using System;
using Qweree.Authentication.WebApi.Infrastructure.Authentication;

namespace Qweree.Authentication.WebApi.Infrastructure.Session;

public class SessionInfoDo
{
    public Guid? Id { get; set; }
    public Guid? ClientId { get; set; }
    public Guid? UserId { get; set; }
    public string? RefreshToken { get; set; }
    public DeviceInfoDo? Device { get; set; }
    public UserAgentInfoDo? UserAgent { get; set; }
    public string? Grant { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? IssuedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
}