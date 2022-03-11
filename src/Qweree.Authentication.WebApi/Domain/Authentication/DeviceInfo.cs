namespace Qweree.Authentication.WebApi.Domain.Authentication;

public class DeviceInfo
{
    public static DeviceInfo Bot(string client)
    {
        return new DeviceInfo(client, "bot", string.Empty, string.Empty, string.Empty);
    }

    public DeviceInfo(string client, string os, string device, string brand, string model)
    {
        Client = client;
        Os = os;
        Device = device;
        Brand = brand;
        Model = model;
    }

    public string Client { get; }
    public string Os { get; }
    public string Device { get; }
    public string Brand { get; }
    public string Model { get; }
}