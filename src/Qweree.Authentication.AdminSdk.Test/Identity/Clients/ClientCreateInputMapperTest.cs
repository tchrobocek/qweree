using System;
using System.Collections.Immutable;
using DeepEqual.Syntax;
using Qweree.Authentication.AdminSdk.Identity.Clients;
using Qweree.TestUtils.DeepEqual;
using Xunit;

namespace Qweree.Authentication.AdminSdk.Test.Identity.Clients
{
    [Trait("Category", "Unit test")]
    public class ClientCreateInputMapperTest
    {

        [Fact]
        public void TestMapper()
        {
            var expected = new ClientCreateInput(Guid.NewGuid(), "client id", "application", "origin",
                Guid.NewGuid(), new[] {Guid.NewGuid(), Guid.NewGuid()}.ToImmutableArray(), new[] {Guid.NewGuid(), Guid.NewGuid()}.ToImmutableArray());
            var dto = ClientCreateInputMapper.ToDto(expected);
            var actual = ClientCreateInputMapper.FromDto(dto);

            actual.WithDeepEqual(expected)
                .WithCustomComparison(new ImmutableArrayComparison())
                .Assert();
        }
    }
}