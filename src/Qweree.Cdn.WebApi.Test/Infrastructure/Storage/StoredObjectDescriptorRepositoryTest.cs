using System.Linq;
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
        public async Task TestInsertAndGetBySlug()
        {
            var descriptor = StoredObjectDescriptorFactory.CreateDefault("object/with/slug", MediaTypeNames.Application.Octet, 0L);
            await _repository.InsertAsync(descriptor);
            var actual = await _repository.GetBySlugAsync(descriptor.Slug.ToArray());

            actual.WithDeepEqual(descriptor)
                .WithCustomComparison(new MillisecondDateTimeComparison())
                .WithCustomComparison(new ImmutableArrayComparison())
                .Assert();
        }

        [Fact]
        public async Task TestFindInSlugAsync()
        {
            var descriptors = new[]
            {
                StoredObjectDescriptorFactory.CreateDefault("/object/a/a/a", MediaTypeNames.Application.Octet, 1L),
                StoredObjectDescriptorFactory.CreateDefault("/object/a/a/b", MediaTypeNames.Application.Octet, 1L),
                StoredObjectDescriptorFactory.CreateDefault("/object/a/a/c", MediaTypeNames.Application.Octet, 1L),
                StoredObjectDescriptorFactory.CreateDefault("/object/a/b", MediaTypeNames.Application.Octet, 1L),
                StoredObjectDescriptorFactory.CreateDefault("/object/a/c", MediaTypeNames.Application.Octet, 1L),
                StoredObjectDescriptorFactory.CreateDefault("/object/a/d", MediaTypeNames.Application.Octet, 1L),
            };

            foreach (var descriptor in descriptors)
            {
                await _repository.InsertAsync(descriptor);
            }

            var expecting = new[]
            {
                new StoredPathDescriptorDo
                {
                    Id = new[] {"object", "a", "b"},
                    FirstId = descriptors[3].Id,
                    FirstMediaType = descriptors[3].MediaType,
                    FirstSize = descriptors[3].Size,
                    FirstCreatedAt = descriptors[3].CreatedAt,
                    FirstModifiedAt = descriptors[3].ModifiedAt,
                    TotalCount = 1,
                    TotalSize = 1,
                    LastCreatedAt = descriptors[3].CreatedAt,
                    LastModifiedAt = descriptors[3].ModifiedAt,
                },
                new StoredPathDescriptorDo
                {
                    Id = new[] {"object", "a", "a"},
                    FirstId = descriptors.First().Id,
                    FirstMediaType = descriptors.First().MediaType,
                    FirstSize = descriptors.First().Size,
                    FirstCreatedAt = descriptors.First().CreatedAt,
                    FirstModifiedAt = descriptors.First().ModifiedAt,
                    TotalCount = 3,
                    TotalSize = 3,
                    LastCreatedAt = descriptors.Last().CreatedAt,
                    LastModifiedAt = descriptors.Last().ModifiedAt,
                },
                new StoredPathDescriptorDo
                {
                    Id = new[] {"object", "a", "d"},
                    FirstId = descriptors[5].Id,
                    FirstMediaType = descriptors[5].MediaType,
                    FirstSize = descriptors[5].Size,
                    FirstCreatedAt = descriptors[5].CreatedAt,
                    FirstModifiedAt = descriptors[5].ModifiedAt,
                    TotalCount = 1,
                    TotalSize = 1,
                    LastCreatedAt = descriptors[5].CreatedAt,
                    LastModifiedAt = descriptors[5].ModifiedAt,
                },
                new StoredPathDescriptorDo
                {
                    Id = new[] {"object", "a", "c"},
                    FirstId = descriptors[4].Id,
                    FirstMediaType = descriptors[4].MediaType,
                    FirstSize = descriptors[4].Size,
                    FirstCreatedAt = descriptors[4].CreatedAt,
                    FirstModifiedAt = descriptors[4].ModifiedAt,
                    TotalCount = 1,
                    TotalSize = 1,
                    LastCreatedAt = descriptors[4].CreatedAt,
                    LastModifiedAt = descriptors[4].ModifiedAt,
                }
            };

            var paths = await _repository.FindInSlugAsync(new[] {"object", "a"});
            paths.WithDeepEqual(expecting)
                .WithCustomComparison(new MillisecondDateTimeComparison())
                .Assert();
        }
    }
}