using System.Linq;
using DeepEqual.Syntax;
using Qweree.Authentication.AdminSdk.Identity.Clients;
using Qweree.Authentication.AdminSdk.Identity.Users;
using Qweree.Authentication.AdminSdk.Test.TestUtils;
using Xunit;

namespace Qweree.Authentication.AdminSdk.Test.Identity.Clients
{
    [Trait("Category", "Unit test")]
    public class CreatedClientMapperTest
    {
        [Fact]
        public void TestMapper()
        {
            var expected = ClientFactory.CreateValidCreatedClient();
            var dto = CreatedClientMapper.ToDto(expected);
            var actual = CreatedClientMapper.FromDto(dto);

            actual.WithDeepEqual(expected)
                .IgnoreProperty(p => p.DeclaringType == typeof(User) && p.Name == nameof(User.Roles))
                .Assert();

            Assert.Equal(expected.Owner.Roles.ToArray(), actual.Owner.Roles.ToArray());
        }
    }
}