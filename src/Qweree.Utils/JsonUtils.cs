using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Qweree.Utils;

public static class JsonUtils
{
    public static readonly JsonSerializerOptions CamelCaseOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
    };

    public static readonly JsonSerializerOptions SnakeCaseNamingPolicy = new()
    {
        PropertyNamingPolicy = new SnakeCaseNamingPolicy(),
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
    };

    public static readonly JsonSerializerOptions NullNamingPolicyOptions = new()
    {
        PropertyNamingPolicy = null,
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
    };

    public static string Serialize(object value)
    {
        return JsonSerializer.Serialize(value, CamelCaseOptions);
    }

    public static string Serialize(object value, JsonSerializerOptions jsonSerializerOptions)
    {
        return JsonSerializer.Serialize(value, jsonSerializerOptions);
    }

    public static async Task SerializeAsync(Stream stream, object value,
        CancellationToken cancellationToken = new())
    {
        await JsonSerializer.SerializeAsync(stream, value, CamelCaseOptions, cancellationToken);
    }

    public static async Task SerializeAsync(Stream stream, object value,
        JsonSerializerOptions jsonSerializerOptions, CancellationToken cancellationToken = new())
    {
        await JsonSerializer.SerializeAsync(stream, value, jsonSerializerOptions, cancellationToken);
    }

    public static object? Deserialize(string json, Type returnType)
    {
        return JsonSerializer.Deserialize(json, returnType, CamelCaseOptions);
    }

    public static TValueType? Deserialize<TValueType>(string json) where TValueType : class
    {
        return JsonSerializer.Deserialize<TValueType>(json, CamelCaseOptions);
    }

    public static TValueType? Deserialize<TValueType>(string json, JsonSerializerOptions jsonSerializerOptions) where TValueType : class
    {
        return JsonSerializer.Deserialize<TValueType>(json, jsonSerializerOptions);
    }

    public static async Task<object?> DeserializeAsync(Stream stream, Type returnType,
        JsonSerializerOptions jsonSerializerOptions, CancellationToken cancellationToken = new())
    {
        return await JsonSerializer.DeserializeAsync(stream, returnType, jsonSerializerOptions, cancellationToken);
    }


    public static async Task<object?> DeserializeAsync(Stream stream, Type returnType,
        CancellationToken cancellationToken = new())
    {
        return await JsonSerializer.DeserializeAsync(stream, returnType, CamelCaseOptions, cancellationToken);
    }

    public static async Task<TValueType?> DeserializeAsync<TValueType>(Stream stream,
        CancellationToken cancellationToken = new()) where TValueType : class
    {
        return await JsonSerializer.DeserializeAsync<TValueType>(stream, CamelCaseOptions, cancellationToken);
    }

    public static async Task<TValueType?> DeserializeAsync<TValueType>(Stream stream,
        JsonSerializerOptions jsonSerializerOptions, CancellationToken cancellationToken = new())
        where TValueType : class
    {
        return await JsonSerializer.DeserializeAsync<TValueType>(stream, jsonSerializerOptions, cancellationToken);
    }
}