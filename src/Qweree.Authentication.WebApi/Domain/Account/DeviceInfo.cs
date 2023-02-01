using System;

namespace Qweree.Authentication.WebApi.Domain.Account;

public class DeviceInfo
{
    public DeviceInfo(Guid id, string client, string os, string device, string brand, string model, DateTime issuedAt, DateTime expiresAt)
    {
        Id = id;
        Client = client;
        Os = os;
        Device = device;
        Brand = brand;
        Model = model;
        IssuedAt = issuedAt;
        ExpiresAt = expiresAt;
    }

    public Guid Id { get; }
    public string Client { get; }
    public string Os { get; }
    public string Device { get; }
    public string Brand { get; }
    public string Model { get; }
    public DateTime IssuedAt { get; }
    public DateTime ExpiresAt { get; }
}