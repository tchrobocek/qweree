using DeepEqual.Syntax;
using Qweree.Authentication.AdminSdk.Identity.Clients;
using Qweree.Authentication.AdminSdk.Test.TestUtils;
using Qweree.TestUtils.DeepEqual;
using Xunit;

namespace Qweree.Authentication.AdminSdk.Test.Identity.Clients
{
    [Trait("Category", "Unit test")]
    public class ClientMapperTest
    {
        [Fact]
        public void TestMapper()
        {
            var expected = ClientFactory.CreateValid();
            var dto = ClientMapper.ToDto(expected);
            var actual = ClientMapper.FromDto(dto);

            actual.WithDeepEqual(expected)
                .WithCustomComparison(new ImmutableArrayComparison())
                .Assert();
        }
    }
}