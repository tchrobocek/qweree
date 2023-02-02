namespace Qweree.Authentication.Sdk.Account.MyAccount;


public class UserAgentInfo
{
    public IClientInfo? Client { get; set; }
    public OperationSystemInfo? OperationSystem { get; set; }
    public string? Device { get; set;  }
    public string? Brand { get; set;  }
    public string? Model { get; set; }
}

public class OperationSystemInfo
{
    public string? Name { get; set; }
    public string? ShortName { get; set; }
    public string? Version { get; set; }
    public string? Platform { get; set; }
}

public interface IClientInfo
{
}

public class BrowserClientInfo : IClientInfo
{
    public string? Name { get; set; }
    public string? Version { get; set; }
    public string? ShortName { get; set; }
    public string? Engine { get; set; }
    public string? EngineVersion { get; set; }
}

public class BotClientInfo : IClientInfo
{
    public string? Name { get; set; }
}