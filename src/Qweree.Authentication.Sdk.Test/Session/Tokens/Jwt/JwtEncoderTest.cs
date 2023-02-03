using System;
using System.Linq;
using System.Security.Cryptography;
using DeepEqual.Syntax;
using Microsoft.IdentityModel.Tokens;
using Qweree.Authentication.Sdk.Identity;
using Qweree.Authentication.Sdk.Session;
using Qweree.Authentication.Sdk.Session.Tokens;
using Qweree.Authentication.Sdk.Session.Tokens.Jwt;
using Qweree.TestUtils.DeepEqual;
using Xunit;

namespace Qweree.Authentication.Sdk.Test.Session.Tokens.Jwt;

public class JwtEncoderTest
{
    [Fact]
    public void TestEncoder()
    {
        var encoder = new JwtEncoder("net.qweree");

        var properties = new[]
        {
            new AuthUserProperty
            {
                Key = "property.hello.xxyyy",
                Value = "hey"
            },
            new AuthUserProperty
            {
                Key = "+ěščřžýáíé=",
                Value = "foo"
            },
            new AuthUserProperty
            {
                Key = "+12345678=",
                Value = "bar"
            },
            new AuthUserProperty
            {
                Key = "+12345678=",
                Value = "baz"
            }
        };
        var identity = new Sdk.Session.Identity
        {
            Client = new IdentityClient
            {
                Id = Guid.NewGuid(),
                ApplicationName = "client",
                ClientId = "client"
            },
            User = new IdentityUser()
            {
                Id = Guid.NewGuid(),
                Username = "user",
                Properties = properties.ToArray()
            },
            Email = "email",
            Roles = new[] {"role1", "role2"}
        };
        var accessToken = new AccessToken
        {
            SessionId = Guid.NewGuid(),
            Identity = identity,
            IssuedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow + TimeSpan.FromSeconds(1)
        };

        using var rsa = RSA.Create();
        var jwt = encoder.EncodeAccessToken(accessToken, new RsaSecurityKey(rsa));

        Assert.NotEmpty(jwt);

        var actualAccessToken = encoder.DecodeAccessToken(jwt);

        actualAccessToken.WithDeepEqual(accessToken)
            .WithCustomComparison(new ImmutableArrayComparison())
            .WithCustomComparison(new MillisecondDateTimeComparison(1000))
            .Assert();
    }
}