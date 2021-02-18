using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Qweree.AspNet.Application;

namespace Qweree.Qwill.WebApi.Domain.Stories
{
    public class FeedService
    {
        private readonly IPublicationRepository _publicationRepository;
        private readonly PublicationService _publicationService;

        public FeedService(IPublicationRepository publicationRepository, PublicationService publicationService)
        {
            _publicationRepository = publicationRepository;
            _publicationService = publicationService;
        }

        public async Task<CollectionResponse<Story>> GetHomeFeedAsync(int skip, int take, CancellationToken cancellationToken = new())
        {
            IEnumerable<Publication> publications;

            try
            {
                publications = await _publicationRepository.FindAsync(skip, take, new Dictionary<string, int>{{"CreationDate", -1}}, cancellationToken);
            }
            catch (Exception)
            {
                return Response.FailCollection<Story>("Failed to load feed.");
            }

            var stories = new List<Story>();

            foreach (var publication in publications)
            {
                stories.Add(await _publicationService.ToStoryAsync(publication, cancellationToken));
            }

            return Response.Ok((IEnumerable<Story>)stories);
        }
    }
}