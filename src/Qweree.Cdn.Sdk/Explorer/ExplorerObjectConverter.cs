using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Qweree.Cdn.Sdk.Explorer;

public class ExplorerObjectConverter : JsonConverter<IExplorerObjectDto>
{
    private readonly Dictionary<string, Type> _typeMap = new()
    {
        ["file"] = typeof(ExplorerFileDto),
        ["directory"] = typeof(ExplorerDirectoryDto)
    };

    public override IExplorerObjectDto? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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
            return (IExplorerObjectDto?)JsonSerializer.Deserialize(ref readerAtStart, targetType, options);
        }

        throw new NotSupportedException($"{className} can not be deserialized");
    }

    public override void Write(Utf8JsonWriter writer, IExplorerObjectDto value, JsonSerializerOptions options)
    {
        switch (value)
        {
            case null:
                JsonSerializer.Serialize(writer, (IExplorerObjectDto?) null, options);
                break;
            default:
            {
                var type = value.GetType();
                JsonSerializer.Serialize(writer, value, type, options);
                break;
            }
        }
    }
}