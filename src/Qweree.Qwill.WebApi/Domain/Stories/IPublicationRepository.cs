using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Qwill.WebApi.Domain.Persistence;

namespace Qweree.Qwill.WebApi.Domain.Stories
{
    public interface IPublicationRepository : IRepository<Publication>
    {
        Task<IEnumerable<Publication>> FindForChannelAsync(Guid channelId, int skip, int take, Dictionary<string, int> sort, CancellationToken cancellationToken);
    }
}