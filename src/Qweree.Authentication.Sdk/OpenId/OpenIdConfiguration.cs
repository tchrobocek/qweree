using System.Text.Json.Serialization;

namespace Qweree.Authentication.Sdk.OpenId;

public class OpenIdConfiguration
{
    [JsonPropertyName("issuer")]
    public string? Issuer { get; set; }

    [JsonPropertyName("authorization_endpoint")]
    public string? AuthorizationEndpoint { get; set; }

    [JsonPropertyName("token_endpoint")]
    public string? TokenEndpoint { get; set; }

    [JsonPropertyName("user_info_endpoint")]
    public string? UserInfoEndpoint { get; set; }

    [JsonPropertyName("jwks_uri")]
    public string? JwksUri { get; set; }

    [JsonPropertyName("registration_endpoint")]
    public string? RegistrationEndpoint  { get; set; }

    [JsonPropertyName("response_types_supported")]
    public string[]? ResponseTypesSupported  { get; set; }

    [JsonPropertyName("grants_supported")]
    public string[]? GrantTypesSupported  { get; set; }
}