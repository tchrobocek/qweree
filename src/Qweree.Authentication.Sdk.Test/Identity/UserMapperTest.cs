using System;
using DeepEqual.Syntax;
using Qweree.Authentication.Sdk.Identity;
using Qweree.TestUtils.DeepEqual;
using Xunit;

namespace Qweree.Authentication.Sdk.Test.Identity
{
    [Trait("Category", "Unit test")]
    public class UserMapperTest
    {
        [Fact]
        public void TestMapper()
        {
            var expected = new User(Guid.NewGuid(), "username", new []{"role0", "role1"});
            var dto = UserMapper.ToDto(expected);
            var actual = UserMapper.FromDto(dto);

            actual.WithDeepEqual(expected)
                .WithCustomComparison(new ImmutableArrayComparison())
                .Assert();
        }
    }
}