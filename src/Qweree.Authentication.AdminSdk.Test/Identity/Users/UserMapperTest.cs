using DeepEqual.Syntax;
using Qweree.Authentication.AdminSdk.Identity.Users;
using Qweree.Authentication.AdminSdk.Test.TestUtils;
using Qweree.TestUtils.DeepEqual;
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
                .WithCustomComparison(new ImmutableArrayComparison())
                .Assert();
        }
    }
}