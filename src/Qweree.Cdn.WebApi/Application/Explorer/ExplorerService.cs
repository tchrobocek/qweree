using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Qweree.AspNet.Application;
using Qweree.Cdn.Sdk;
using Qweree.Cdn.Sdk.Explorer;
using Qweree.Cdn.WebApi.Infrastructure.Storage;

namespace Qweree.Cdn.WebApi.Application.Explorer
{
    public class ExplorerService
    {
        private readonly IStoredObjectDescriptorRepository _descriptorRepository;

        public ExplorerService(IStoredObjectDescriptorRepository descriptorRepository)
        {
            _descriptorRepository = descriptorRepository;
        }

        public async Task<PaginationResponse<IExplorerObject>> ExplorePathAsync(ExplorerFilter filter,
            CancellationToken cancellationToken = new())
        {
            var slug = SlugHelper.PathToSlug(filter.Path);
            var explorerObjects = (await _descriptorRepository.FindInSlugAsync(slug, cancellationToken))
                .Select(ExplorerObjectMapper.ToExplorerObject)
                .ToArray();

            return Response.Ok(explorerObjects, explorerObjects.Length);
        }
    }
}