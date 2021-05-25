using System;
using System.Linq;
using DeepEqual.Syntax;
using Qweree.Authentication.AdminSdk.Identity.Users;
using Xunit;

namespace Qweree.Authentication.AdminSdk.Test.Identity.Users
{
    [Trait("Category", "Unit test")]
    public class UserCreateInputMapperTest
    {
        [Fact]
        public void TestMapper()
        {
            var expected = new UserCreateInput("username", "contact email", "full name", "password",
                new[] {Guid.NewGuid(), Guid.NewGuid()});
            var dto = UserCreateInputMapper.ToDto(expected);
            var actual = UserCreateInputMapper.FromDto(dto);

            actual.WithDeepEqual(expected)
                .IgnoreProperty(p => p.DeclaringType == typeof(UserCreateInput) && p.Name == nameof(UserCreateInput.Roles))
                .Assert();
            Assert.Equal(expected.Roles.ToArray(), actual.Roles.ToArray());
        }
    }
}