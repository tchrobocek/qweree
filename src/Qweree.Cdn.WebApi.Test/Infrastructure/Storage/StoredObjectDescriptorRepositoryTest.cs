using System.Net.Mime;
using System.Threading.Tasks;
using DeepEqual.Syntax;
using Qweree.Cdn.WebApi.Infrastructure.Storage;
using Qweree.Cdn.WebApi.Test.Fixture;
using Qweree.Cdn.WebApi.Test.Fixture.Factories;
using Qweree.Mongo;
using Qweree.TestUtils.DeepEqual;
using Xunit;

namespace Qweree.Cdn.WebApi.Test.Infrastructure.Storage
{
    [Collection("Database collection")]
    [Trait("Category", "Integration test")]
    [Trait("Category", "Database test")]
    public class StoredObjectDescriptorRepositoryTest
    {
        private readonly StoredObjectDescriptorRepository _repository;

        public StoredObjectDescriptorRepositoryTest()
        {
            var context = new MongoContext(Settings.Database.ConnectionString, Settings.Database.DatabaseName);
            _repository = new StoredObjectDescriptorRepository(context);
            _repository.DeleteAllAsync()
                .GetAwaiter()
                .GetResult();
        }

        [Fact]
        public async Task TestInsertAndGet()
        {
            var descriptor = StoredObjectDescriptorFactory.CreateDefault("object", MediaTypeNames.Application.Octet, 0L);
            await _repository.InsertAsync(descriptor);
            var actual = await _repository.GetAsync(descriptor.Id);

            actual.WithDeepEqual(descriptor)
                .WithCustomComparison(new MillisecondDateTimeComparison())
                .WithCustomComparison(new ImmutableArrayComparison())
                .Assert();
        }
    }
}