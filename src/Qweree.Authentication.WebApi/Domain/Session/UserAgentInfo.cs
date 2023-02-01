namespace Qweree.Authentication.WebApi.Domain.Session;

public class UserAgentInfo
{
    public UserAgentInfo(string userAgentString, IClientInfo? client, OperationSystemInfo? operationSystem, string device, string brand, string model)
    {
        UserAgentString = userAgentString;
        Client = client;
        OperationSystem = operationSystem;
        Device = device;
        Brand = brand;
        Model = model;
    }

    public string UserAgentString { get; }
    public IClientInfo? Client { get; }
    public OperationSystemInfo? OperationSystem { get; }
    public string Device { get; }
    public string Brand { get; }
    public string Model { get; }
}

public class OperationSystemInfo
{
    public OperationSystemInfo(string operationSystemString, string name, string shortName, string version, string platform)
    {
        OperationSystemString = operationSystemString;
        Name = name;
        ShortName = shortName;
        Version = version;
        Platform = platform;
    }

    public string OperationSystemString { get; }
    public string Name { get; }
    public string ShortName { get; }
    public string Version { get; }
    public string Platform { get; }
}

public interface IClientInfo
{
    string ClientString { get; }
}

public class BrowserClientInfo : IClientInfo
{
    public BrowserClientInfo(string clientString, string name, string version, string shortName, string engine,
        string engineVersion)
    {
        ClientString = clientString;
        Name = name;
        Version = version;
        ShortName = shortName;
        Engine = engine;
        EngineVersion = engineVersion;
    }

    public string ClientString { get; }
    public string Name { get; }
    public string Version { get; }
    public string ShortName { get; }
    public string Engine { get; }
    public string EngineVersion { get; }
}

public class BotClientInfo : IClientInfo
{
    public BotClientInfo(string clientString)
    {
        ClientString = clientString;
    }

    public string ClientString { get; }
}