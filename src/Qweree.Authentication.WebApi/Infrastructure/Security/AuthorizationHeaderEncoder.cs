using System;
using System.Text;
using Qweree.Authentication.WebApi.Domain.Authentication;

namespace Qweree.Authentication.WebApi.Infrastructure.Security;

public class AuthorizationHeaderEncoder
{
    private readonly Encoding _encoding = Encoding.GetEncoding("iso-8859-1");

    public string Encode(ClientCredentials clientCredentials)
    {
        var credentials = $"{clientCredentials.ClientId}:{clientCredentials.ClientSecret}";
        var hash = Convert.ToBase64String(_encoding.GetBytes(credentials));
        return hash;
    }

    public ClientCredentials Decode(string hash)
    {
        string clientCredentialsString = _encoding.GetString(Convert.FromBase64String(hash));
        var parts = clientCredentialsString.Split(":");

        if (parts.Length != 2)
        {
            throw new InvalidOperationException("Invalid value.");
        }

        return new ClientCredentials(parts[0], parts[1]);
    }
}