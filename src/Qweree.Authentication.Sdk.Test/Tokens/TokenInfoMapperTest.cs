using System;
using DeepEqual.Syntax;
using Qweree.Authentication.Sdk.OAuth2;
using Xunit;

namespace Qweree.Authentication.Sdk.Test.Tokens;

[Trait("Category", "Unit test")]
public class TokenInfoMapperTest
{
    [Fact]
    public void TestTokenInfoMapper()
    {
        var expected = new TokenInfo("access", "refresh", DateTime.UtcNow);
        var dto = TokenInfoMapper.ToDto(expected);
        var actual = TokenInfoMapper.FromDto(dto);

        actual.ShouldDeepEqual(expected);
    }
}