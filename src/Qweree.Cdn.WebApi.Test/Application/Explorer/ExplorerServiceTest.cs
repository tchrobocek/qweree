using System;
using System.Linq;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using DeepEqual.Syntax;
using Moq;
using Qweree.Cdn.WebApi.Application.Explorer;
using Qweree.Cdn.WebApi.Infrastructure;
using Qweree.Cdn.WebApi.Infrastructure.Storage;
using Qweree.Cdn.WebApi.Test.Fixture.Factories;
using Qweree.TestUtils.DeepEqual;
using Xunit;

namespace Qweree.Cdn.WebApi.Test.Application.Explorer
{
    [Trait("Category", "Unit test")]
    public class ExplorerServiceTest
    {
        [Fact]
        public async Task TestExplore()
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

            var storedObjects = new[]
            {
                new StoredPathDescriptorDo
                {
                    Id = new[] {"object", "a", "b"},
                    FirstId = descriptors[3].Id,
                    FirstSlug = descriptors[3].Slug.ToArray(),
                    FirstMediaType = descriptors[3].MediaType,
                    FirstSize = descriptors[3].Size,
                    FirstCreatedAt = descriptors[3].CreatedAt,
                    FirstModifiedAt = descriptors[3].ModifiedAt,
                    TotalCount = 1,
                    TotalSize = 1,
                    MinCreatedAt = descriptors[3].CreatedAt,
                    MaxModifiedAt = descriptors[3].ModifiedAt,
                },
                new StoredPathDescriptorDo
                {
                    Id = new[] {"object", "a", "a"},
                    FirstId = descriptors.First().Id,
                    FirstSlug = descriptors.First().Slug.ToArray(),
                    FirstMediaType = descriptors.First().MediaType,
                    FirstSize = descriptors.First().Size,
                    FirstCreatedAt = descriptors.First().CreatedAt,
                    FirstModifiedAt = descriptors.First().ModifiedAt,
                    TotalCount = 3,
                    TotalSize = 3,
                    MinCreatedAt = descriptors.First().CreatedAt,
                    MaxModifiedAt = descriptors.First().ModifiedAt,
                },
                new StoredPathDescriptorDo
                {
                    Id = new[] {"object", "a", "d"},
                    FirstId = descriptors[5].Id,
                    FirstSlug = descriptors[5].Slug.ToArray(),
                    FirstMediaType = descriptors[5].MediaType,
                    FirstSize = descriptors[5].Size,
                    FirstCreatedAt = descriptors[5].CreatedAt,
                    FirstModifiedAt = descriptors[5].ModifiedAt,
                    TotalCount = 1,
                    TotalSize = 1,
                    MinCreatedAt = descriptors[5].CreatedAt,
                    MaxModifiedAt = descriptors[5].ModifiedAt,
                },
                new StoredPathDescriptorDo
                {
                    Id = new[] {"object", "a", "c"},
                    FirstId = descriptors[4].Id,
                    FirstSlug = descriptors[4].Slug.ToArray(),
                    FirstMediaType = descriptors[4].MediaType,
                    FirstSize = descriptors[4].Size,
                    FirstCreatedAt = descriptors[4].CreatedAt,
                    FirstModifiedAt = descriptors[4].ModifiedAt,
                    TotalCount = 1,
                    TotalSize = 1,
                    MinCreatedAt = descriptors[4].CreatedAt,
                    MaxModifiedAt = descriptors[4].ModifiedAt,
                }
            }.OrderBy(s => string.Join("/", s.FirstSlug ?? ArraySegment<string?>.Empty))
                .ToArray();


            var expected = new[]
            {
                new ExplorerDirectory("/object/a/a", 3, 3, descriptors.First().CreatedAt, descriptors.First().ModifiedAt),
                ExplorerObjectMapper.ToExplorerObject(storedObjects[1]),
                ExplorerObjectMapper.ToExplorerObject(storedObjects[2]),
                ExplorerObjectMapper.ToExplorerObject(storedObjects[3]),
            };
            var slug = new[] {"object", "a"};

            var repositoryMock = new Mock<IStoredObjectDescriptorRepository>();
            repositoryMock.Setup(m => m.FindInSlugAsync(slug, It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => storedObjects);

            var service = new ExplorerService(repositoryMock.Object);
            var response = await service.ExplorePathAsync(new ExplorerFilter(SlugHelper.SlugToPath(slug)));

            Assert.Equal(expected.Length, response.DocumentCount);
            response.Payload.WithDeepEqual(expected)
                .WithCustomComparison(new ImmutableArrayComparison())
                .Assert();
        }

    }
}