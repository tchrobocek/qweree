using System;
using Qweree.Authentication.Sdk.Identity;

namespace Qweree.Authentication.Sdk.Account.MyAccount;

public class SessionInfo
{
    public Guid? Id { get; set; }
    public Client? Client { get; set; }
    public UserAgentInfo? UserAgent { get; set; }
    public string? Grant { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? IssuedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
}
