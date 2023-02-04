using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Qweree.Authentication.Sdk.Account.MyAccount;

public class ClientInfoConverter : JsonConverter<IClientInfo>
{
    private readonly Dictionary<string, Type> _typeMap = new()
    {
        ["bot"] = typeof(BotClientInfo),
        ["browser"] = typeof(BrowserClientInfo)
    };

    public override IClientInfo? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Check for null values
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        // Copy the current state from reader (it's a struct)
        var readerAtStart = reader;

        // Read the `className` from our JSON document
        using var jsonDocument = JsonDocument.ParseValue(ref reader);
        var jsonObject = jsonDocument.RootElement;

        var className = jsonObject.EnumerateObject().FirstOrDefault(p => p.Name.ToLower() == "typename")
            .Value.GetString();

        if (className is null)
            return null;

        // See if that class can be deserialized or not
        if (!string.IsNullOrEmpty(className) && _typeMap.TryGetValue(className, out var targetType))
        {
            return (IClientInfo?)JsonSerializer.Deserialize(ref readerAtStart, targetType, options);
        }

        throw new NotSupportedException($"{className} can not be deserialized");
    }

    public override void Write(Utf8JsonWriter writer, IClientInfo value, JsonSerializerOptions options)
    {
        switch (value)
        {
            case null:
                JsonSerializer.Serialize(writer, (IClientInfo?) null, options);
                break;
            default:
                var type = value.GetType();
                JsonSerializer.Serialize(writer, value, type, options);
                break;
        }
    }
}