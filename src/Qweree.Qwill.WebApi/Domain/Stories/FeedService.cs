using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Qweree.AspNet.Application;
using Qweree.Mongo;

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

        public async Task<PaginationResponse<Story>> GetHomeFeedAsync(int skip, int take)
        {
            Pagination<Publication> publications;

            try
            {
                publications = await _publicationRepository.PaginateAsync(skip, take, new Dictionary<string, int>{{"CreationDate", -1}});
            }
            catch (Exception)
            {
                return Response.FailPagination<Story>("Failed to load feed.");
            }

            return Response.Ok(publications.Documents.Select(_publicationService.ToStory), publications.TotalCount);
        }
    }
}