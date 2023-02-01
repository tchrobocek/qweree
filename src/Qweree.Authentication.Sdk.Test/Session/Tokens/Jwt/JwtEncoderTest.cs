using System;
using System.Linq;
using DeepEqual.Syntax;
using Qweree.Authentication.Sdk.Session;
using Qweree.Authentication.Sdk.Session.Tokens;
using Qweree.Authentication.Sdk.Session.Tokens.Jwt;
using Qweree.Authentication.Sdk.Users;
using Qweree.TestUtils.DeepEqual;
using Xunit;

namespace Qweree.Authentication.Sdk.Test.Session.Tokens.Jwt;

public class JwtEncoderTest
{
    [Fact]
    public void TestEncoder()
    {
        var encoder = new JwtEncoder("net.qweree", "qweree", "$2a$06$Kkim544a5cO/dHyknpvH3eMTpe1sg8iMx6dWUhEiTFUV.BfXNPmVG");

        var properties = new[]
        {
            new UserProperty
            {
                Key = "property.hello.xxyyy",
                Value = "hey"
            },
            new UserProperty
            {
                Key = "+ěščřžýáíé=",
                Value = "foo"
            },
            new UserProperty
            {
                Key = "+12345678=",
                Value = "bar"
            },
            new UserProperty
            {
                Key = "+12345678=",
                Value = "baz"
            }
        };
        var identity = new Identity
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
        var jwt = encoder.EncodeAccessToken(accessToken);

        Assert.NotEmpty(jwt);

        var actualAccessToken = encoder.DecodeAccessToken(jwt);

        actualAccessToken.WithDeepEqual(accessToken)
            .WithCustomComparison(new ImmutableArrayComparison())
            .WithCustomComparison(new MillisecondDateTimeComparison(1000))
            .Assert();
    }
}