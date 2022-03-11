using System;

namespace Qweree.Authentication.Sdk.Account;

public class DeviceInfoDto
{
    public Guid? Id { get; set; }
    public string? Client { get; set; }
    public string? Os { get; set; }
    public string? Device { get; set; }
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public DateTime? IssuedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
}