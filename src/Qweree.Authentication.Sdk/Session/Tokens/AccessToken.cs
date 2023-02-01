using System;

namespace Qweree.Authentication.Sdk.Session.Tokens;

public class AccessToken
{
    public Guid? SessionId { get; set; }
    public Identity? Identity { get; set; }
    public DateTime? IssuedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
}