using System;
using Qweree.Authentication.Sdk.Identity;

namespace Qweree.Authentication.Sdk.Account.MyAccount;

public class MyAccountSessionInfo
{
    public Guid? Id { get; set; }
    public AuthClient? Client { get; set; }
    public AuthUserAgentInfo? UserAgent { get; set; }
    public string? Grant { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? IssuedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
}
