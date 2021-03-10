using DeepEqual.Syntax;
using Qweree.Authentication.AdminSdk.Identity.Users;
using Qweree.TestUtils.DeepEqual;
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
                new[] {"role1", "role2"});
            var dto = UserCreateInputMapper.ToDto(expected);
            var actual = UserCreateInputMapper.FromDto(dto);

            actual.WithDeepEqual(expected)
                .WithCustomComparison(new ImmutableArrayComparison())
                .Assert();
        }
    }
}