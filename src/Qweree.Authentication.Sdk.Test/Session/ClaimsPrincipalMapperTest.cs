using System;
using System.Collections.Immutable;
using DeepEqual.Syntax;
using Qweree.Authentication.Sdk.Session;
using Qweree.Authentication.Sdk.Users;
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
            new UserProperty("property.hello.xxyyy", "hey"),
            new UserProperty("+ěščřžýáíé=", "foo"),
            new UserProperty("+12345678=", "bar"),
            new UserProperty("+12345678=", "baz"),
        };
        var identity = new Identity(new IdentityClient(Guid.NewGuid(), "client", "app"),
            new(Guid.NewGuid(), "user", "full name", properties.ToImmutableArray()),
            "email", ImmutableArray<string>.Empty);
        var claimsPrincipal = IdentityMapper.FromDto(identity);
        var actualIdentity = IdentityMapper.ToIdentity(claimsPrincipal);

        actualIdentity.WithDeepEqual(identity)
            .WithCustomComparison(new ImmutableArrayComparison())
            .Assert();
    }
}