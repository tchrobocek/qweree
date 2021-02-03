using Qweree.Authentication.WebApi.Infrastructure.Security;
using Xunit;

namespace Qweree.Authentication.WebApi.Test.Infrastructure.Security
{
    public class BCryptPasswordEncoderTest
    {
        [Fact]
        public void TestEncode()
        {
            var encoder = new BCryptPasswordEncoder();
            var encoded1 = encoder.EncodePassword("password");
            var encoded2 = encoder.EncodePassword("password");

            Assert.NotEmpty(encoded1);
            Assert.NotEmpty(encoded2);
            Assert.NotEqual(encoded1, encoded2);
            Assert.True(encoder.VerifyPassword(encoded1, "password"));
            Assert.True(encoder.VerifyPassword(encoded2, "password"));
            Assert.False(encoder.VerifyPassword(encoded1, "x"));
            Assert.False(encoder.VerifyPassword(encoded2, "x"));
        }
    }
}