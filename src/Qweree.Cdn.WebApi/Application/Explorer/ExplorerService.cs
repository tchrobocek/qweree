using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Qweree.AspNet.Application;
using Qweree.Cdn.WebApi.Application.Storage;
using Qweree.Cdn.WebApi.Infrastructure;
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

        public async Task<PaginationResponse<IExplorerObject>> ExplorePathAsync(ExplorerFilter filter, CancellationToken cancellationToken = new CancellationToken())
        {
            var slug = SlugHelper.PathToSlug(filter.Path);
            var explorerObjects = (await _descriptorRepository.FindInSlugAsync(slug, cancellationToken))
                .Select(ExplorerObjectMapper.ToExplorerObject)
                .ToArray();

            return Response.Ok(explorerObjects, explorerObjects.Length);
        }
    }
}