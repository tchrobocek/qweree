using System.Text.Json;
using System.Text.Json.Serialization;
using Qweree.Authentication.AdminSdk.Identity.Clients;
using Qweree.Utils;

namespace Qweree.Authentication.AdminSdk;

internal static class JsonHelper
{
    public static readonly JsonSerializerOptions CamelCaseOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
        Converters =
        {
            new AccessDefinitionInputConverter()
        }
    };

    public static string Serialize(object obj)
    {
        return JsonUtils.Serialize(obj, CamelCaseOptions);
    }
}