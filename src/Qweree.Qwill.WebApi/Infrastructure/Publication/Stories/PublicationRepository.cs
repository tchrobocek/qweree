using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Mongo;
using Qweree.Qwill.WebApi.Domain.Stories;

namespace Qweree.Qwill.WebApi.Infrastructure.Publication.Stories
{
    public class PublicationRepository : MongoRepositoryBase<Domain.Stories.Publication, PublicationDo>, IPublicationRepository
    {
        public PublicationRepository(MongoContext context) : base("publications", context)
        {
        }

        protected override Func<Domain.Stories.Publication, PublicationDo> ToDocument => PublicationMapper.ToDo;
        protected override Func<PublicationDo, Domain.Stories.Publication> FromDocument => PublicationMapper.FromDo;
        public Task<IEnumerable<Domain.Stories.Publication>> FindForChannelAsync(Guid channelId, int skip, int take, Dictionary<string, int> sort, CancellationToken cancellationToken)
        {
            var query = $@"{{""ChannelId"": UUID(""{channelId}"")}}";
            return FindAsync(query, skip, take, sort, cancellationToken);
        }
    }
}