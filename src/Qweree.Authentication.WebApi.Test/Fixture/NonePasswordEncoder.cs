using System;
using Qweree.Authentication.WebApi.Domain.Security;

namespace Qweree.Authentication.WebApi.Test.Fixture;

public class NonePasswordEncoder : IPasswordEncoder
{
    public string EncodePassword(string password)
    {
        return password;
    }

    public bool VerifyPassword(string hash, string raw)
    {
        return hash.Equals(raw, StringComparison.InvariantCulture);
    }
}