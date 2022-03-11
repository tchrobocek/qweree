using System;

namespace Qweree.Authentication.Sdk.Account;

public class DeviceInfo
{
    public DeviceInfo(Guid id, string client, string os, string device, string brand, string model)
    {
        Id = id;
        Client = client;
        Os = os;
        Device = device;
        Brand = brand;
        Model = model;
    }

    public Guid Id { get; }
    public string Client { get; }
    public string Os { get; }
    public string Device { get; }
    public string Brand { get; }
    public string Model { get; }
}