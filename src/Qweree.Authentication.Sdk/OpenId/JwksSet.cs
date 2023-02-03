using System;
using System.Text.Json.Serialization;

namespace Qweree.Authentication.Sdk.OpenId;

public class JwksSet
{
    [JsonPropertyName("keys")]
    public Jwk[]? Keys { get; set; }
}

public class Jwk
{
    [JsonPropertyName("kty")]
    public string KeyType { get; set; } = string.Empty;

    [JsonPropertyName("use")]
    public string Use { get; set; } = string.Empty;

    [JsonPropertyName("n")]
    public string Modulus { get; set; } = string.Empty;

    [JsonPropertyName("e")]
    public string Exponent { get; set; } = string.Empty;

    [JsonPropertyName("x5c")]
    public string[] CertificateChain { get; set; } = Array.Empty<string>();
}