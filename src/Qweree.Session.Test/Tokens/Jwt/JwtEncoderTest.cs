using System;
using System.Collections.Immutable;
using DeepEqual.Syntax;
using Qweree.Session.Tokens;
using Qweree.Session.Tokens.Jwt;
using Qweree.TestUtils.DeepEqual;
using Xunit;

namespace Qweree.Session.Test.Tokens.Jwt;

public class JwtEncoderTest
{
    [Fact]
    public void TestEncoder()
    {
        var encoder = new JwtEncoder("net.qweree", "qweree", "$2a$06$Kkim544a5cO/dHyknpvH3eMTpe1sg8iMx6dWUhEiTFUV.BfXNPmVG");

        var identity = new Identity(new(Guid.NewGuid(), "client", "app"), new(Guid.NewGuid(), "user", "full name"), "email", new[] {"role1", "role2"}.ToImmutableArray());
        var accessToken = new AccessToken(identity, DateTime.UtcNow, DateTime.UtcNow + TimeSpan.FromSeconds(1));
        var jwt = encoder.EncodeAccessToken(accessToken);

        Assert.NotEmpty(jwt);

        var actualAccessToken = encoder.DecodeAccessToken(jwt);

        actualAccessToken.WithDeepEqual(accessToken)
            .WithCustomComparison(new ImmutableArrayComparison())
            .WithCustomComparison(new MillisecondDateTimeComparison(1000))
            .Assert();
    }
}