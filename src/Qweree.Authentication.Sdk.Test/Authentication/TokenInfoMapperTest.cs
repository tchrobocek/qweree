using System;
using DeepEqual.Syntax;
using Qweree.Authentication.Sdk.Authentication;
using Xunit;

namespace Qweree.Authentication.Sdk.Test.Authentication
{
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
}