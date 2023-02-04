using System;
using Qweree.Authentication.AdminSdk.Identity.Clients;
using Qweree.Authentication.AdminSdk.Identity.Users;

namespace Qweree.Authentication.AdminSdk.Session;

public class SessionInfo
{
    public Guid? Id { get; set; }
    public Client? Client { get; set; }
    public User? User { get; set; }
    public string? IpAddress { get; set; }
    public UserAgentInfo? UserAgent { get; set; }
    public string? Grant { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? IssuedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
}