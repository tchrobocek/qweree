using DeepEqual.Syntax;
using Qweree.Authentication.WebApi.Domain.Authentication;
using Qweree.Authentication.WebApi.Infrastructure.Security;
using Xunit;

namespace Qweree.Authentication.WebApi.Test.Infrastructure.Security;

public class AuthorizationHeaderEncoderTest
{
    [Fact]
    public void TestEncodeDecode()
    {
        var credentials = new ClientCredentials("id", "password");
        var encoder = new AuthorizationHeaderEncoder();
        var hash = encoder.Encode(credentials);
        var actualCredentials = encoder.Decode(hash);

        actualCredentials.ShouldDeepEqual(credentials);
    }
}