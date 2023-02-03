using MongoDB.Bson.Serialization.Attributes;

namespace Qweree.Authentication.WebApi.Infrastructure.Session;

public class UserAgentInfoDo
{
    public string? UserAgentString { get; set; }
    public ClientInfoDo? Client { get; set; }
    public OperationSystemInfoDo? OperationSystem { get; set; }
    public string? Device { get; set; }
    public string? Brand { get; set; }
    public string? Model { get; set; }
}

public class OperationSystemInfoDo
{
    public string? OperationSystemString { get; set; }
    public string? Name { get; set; }
    public string? ShortName { get; set; }
    public string? Version { get; set; }
    public string? Platform { get; set; }
}

[BsonKnownTypes(typeof(BrowserClientInfoDo), typeof(BotClientInfoDo))]
public abstract class ClientInfoDo
{
    public string? ClientString { get; set; }
}

[BsonDiscriminator("browser")]
public class BrowserClientInfoDo : ClientInfoDo
{
    public string? Name { get; set; }
    public string? Version { get; set; }
    public string? ShortName { get; set; }
    public string? Engine { get; set; }
    public string? EngineVersion { get; set; }
}

[BsonDiscriminator("bot")]
public class BotClientInfoDo : ClientInfoDo
{
}