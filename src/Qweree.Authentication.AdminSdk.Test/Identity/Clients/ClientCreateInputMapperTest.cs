using System;
using DeepEqual.Syntax;
using Qweree.Authentication.AdminSdk.Identity.Clients;
using Xunit;

namespace Qweree.Authentication.AdminSdk.Test.Identity.Clients
{
    [Trait("Category", "Unit test")]
    public class ClientCreateInputMapperTest
    {

        [Fact]
        public void TestMapper()
        {
            var expected = new ClientCreateInput(Guid.NewGuid(), "client id", "client secret", "application", "origin",
                Guid.NewGuid());
            var dto = ClientCreateInputMapper.ToDto(expected);
            var actual = ClientCreateInputMapper.FromDto(dto);

            actual.WithDeepEqual(expected)
                .Assert();
        }
    }
}