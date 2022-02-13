using System;
using System.Collections.Immutable;
using DeepEqual.Syntax;
using Qweree.Authentication.Sdk.Session;
using Xunit;

namespace Qweree.Authentication.Sdk.Test.Session;

public class ClaimsPrincipalMapperTest
{
    [Fact]
    public void TestMap()
    {
        var identity = new Identity(new(Guid.NewGuid(), "client", "app"), new(Guid.NewGuid(), "user", "full name"), "email", ImmutableArray<string>.Empty);
        var claimsPrincipal = IdentityMapper.FromDto(identity);
        var actualIdentity = IdentityMapper.ToIdentity(claimsPrincipal);

        actualIdentity.ShouldDeepEqual(identity);
    }
}