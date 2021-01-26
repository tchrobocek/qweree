using System;
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
    }
}