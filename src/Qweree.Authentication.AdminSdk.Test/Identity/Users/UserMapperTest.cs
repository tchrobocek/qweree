using System.Linq;
using DeepEqual.Syntax;
using Qweree.Authentication.AdminSdk.Identity.Users;
using Qweree.Authentication.AdminSdk.Test.TestUtils;
using Xunit;

namespace Qweree.Authentication.AdminSdk.Test.Identity.Users
{
    [Trait("Category", "Unit test")]
    public class UserMapperTest
    {
        [Fact]
        public void TestMapper()
        {
            var expected = UserFactory.CreateValid();
            var dto = UserMapper.ToDto(expected);
            var actual = UserMapper.FromDto(dto);

            actual.WithDeepEqual(expected)
                .IgnoreProperty(p => p.DeclaringType == typeof(User) && p.Name == nameof(User.Roles))
                .Assert();

            Assert.Equal(expected.Roles.ToArray(), actual.Roles.ToArray());
        }
    }
}