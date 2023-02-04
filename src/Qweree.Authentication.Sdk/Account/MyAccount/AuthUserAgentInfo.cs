namespace Qweree.Authentication.Sdk.Account.MyAccount;


public class AuthUserAgentInfo
{
    public IAuthClientInfo? Client { get; set; }
    public AuthOperationSystemInfo? OperationSystem { get; set; }
    public string? Device { get; set;  }
    public string? Brand { get; set;  }
    public string? Model { get; set; }
}

public class AuthOperationSystemInfo
{
    public string? Name { get; set; }
    public string? ShortName { get; set; }
    public string? Version { get; set; }
    public string? Platform { get; set; }
}

public interface IAuthClientInfo
{
    public string TypeName { get; }
}

public class BrowserAuthClientInfo : IAuthClientInfo
{
    public string TypeName => "browser";
    public string? Name { get; set; }
    public string? Version { get; set; }
    public string? ShortName { get; set; }
    public string? Engine { get; set; }
    public string? EngineVersion { get; set; }
}

public class BotAuthClientInfo : IAuthClientInfo
{
    public string TypeName => "bot";
    public string? Name { get; set; }
}