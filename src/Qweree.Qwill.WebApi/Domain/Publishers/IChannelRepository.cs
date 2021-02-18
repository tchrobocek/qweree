using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Qwill.WebApi.Domain.Persistence;

namespace Qweree.Qwill.WebApi.Domain.Publishers
{
    public interface IChannelRepository : IRepository<Channel>
    {
        Task<IEnumerable<Channel>> FindUserAuthorAsync(Guid userId, CancellationToken cancellationToken = new());
    }
}