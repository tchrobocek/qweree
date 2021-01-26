using Xunit;

namespace Qweree.Cdn.WebApi.Test.Fixture
{
    [CollectionDefinition("Web api collection")]
    public class WebApiCollection : ICollectionFixture<WebApiFactory>
    {
    }
}