using System;
using DeepEqual.Syntax;
using Qweree.Authentication.WebApi.Domain.Authentication;
using Qweree.Authentication.WebApi.Infrastructure.Authentication;
using Xunit;

namespace Qweree.Authentication.WebApi.Test.Infrastructure.Authentication
{
    [Trait("Category", "Unit test")]
    public class RefreshTokenMapperTest
    {
        [Fact]
        public void TestToAndFromDo()
        {
            var refreshToken =
                new RefreshToken(Guid.NewGuid(), "token", Guid.NewGuid(), DateTime.UtcNow, DateTime.UtcNow);
            var refreshTokenDo = RefreshTokenMapper.ToDo(refreshToken);
            var actualRefreshToken = RefreshTokenMapper.FromDo(refreshTokenDo);

            actualRefreshToken.ShouldDeepEqual(refreshToken);
        }
    }
}