using System;
using Qweree.Cdn.WebApi.Infrastructure.Storage;
using Qweree.Cdn.WebApi.Test.Fixture.Factories;
using Xunit;

namespace Qweree.Cdn.WebApi.Test.Infrastructure.Storage;

[Trait("Category", "Unit test")]
public class ETagHelperTest
{
    [Fact]
    public void TestHelper()
    {
        var descriptor = StoredObjectDescriptorFactory.CreateDefault();
        var etag = EtagHelper.ComputeEtag(descriptor);

        Assert.True(EtagHelper.ValidateEtag(etag, descriptor));
    }

    [Fact]
    public void TestHelper_NotValid()
    {
        var descriptor1 = StoredObjectDescriptorFactory.CreateDefault(DateTime.Now);
        var descriptor2 = StoredObjectDescriptorFactory.CreateDefault(DateTime.Now - TimeSpan.FromSeconds(1));
        var etag = EtagHelper.ComputeEtag(descriptor1);

        Assert.False(EtagHelper.ValidateEtag(etag, descriptor2));
    }
}