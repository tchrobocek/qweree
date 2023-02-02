using System;
using System.Linq;
using DeepEqual.Syntax;
using Qweree.Authentication.Sdk.Identity;
using Qweree.Authentication.Sdk.Session;
using Qweree.TestUtils.DeepEqual;
using Xunit;

namespace Qweree.Authentication.Sdk.Test.Session;

public class ClaimsPrincipalMapperTest
{
    [Fact]
    public void TestMap()
    {
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
        var claimsPrincipal = IdentityMapper.ToClaimsPrincipal(identity);
        var actualIdentity = IdentityMapper.ToIdentity(claimsPrincipal);

        actualIdentity.WithDeepEqual(identity)
            .WithCustomComparison(new ImmutableArrayComparison())
            .Assert();
    }
}