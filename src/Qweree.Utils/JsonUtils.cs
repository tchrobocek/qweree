using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Qweree.Utils
{
    public static class JsonUtils
    {
        private static readonly JsonSerializerOptions CamelCaseOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        public static string Serialize(object value)
        {
            return JsonSerializer.Serialize(value, CamelCaseOptions);
        }

        public static async Task SerializeAsync(Stream stream, object value, CancellationToken cancellationToken = new CancellationToken())
        {
            await JsonSerializer.SerializeAsync(stream, value, CamelCaseOptions, cancellationToken);
        }

        public static object? Deserialize(string json, Type returnType)
        {
            return JsonSerializer.Deserialize(json, returnType, CamelCaseOptions);
        }

        public static TValueType? Deserialize<TValueType>(string json) where TValueType : class
        {
            return JsonSerializer.Deserialize<TValueType>(json, CamelCaseOptions);
        }

        public static async Task<object?> DeserializeAsync(Stream stream, Type returnType, CancellationToken cancellationToken = new CancellationToken())
        {
            return await JsonSerializer.DeserializeAsync(stream, returnType, CamelCaseOptions, cancellationToken);
        }

        public static async Task<TValueType?> DeserializeAsync<TValueType>(Stream stream, CancellationToken cancellationToken = new CancellationToken()) where TValueType : class
        {
            return await JsonSerializer.DeserializeAsync<TValueType>(stream, CamelCaseOptions, cancellationToken);
        }
    }
}