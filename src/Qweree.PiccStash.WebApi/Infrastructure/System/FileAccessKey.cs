using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace Qweree.PiccStash.WebApi.Infrastructure.System;

public class FileAccessKey
{
    private readonly byte[] _key;

    public FileAccessKey()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        _key = SHA256.HashData(bytes);
    }

    public SymmetricSecurityKey GetKey()
    {
        return new SymmetricSecurityKey(_key);
    }
}