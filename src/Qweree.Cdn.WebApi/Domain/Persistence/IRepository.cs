using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Qweree.Cdn.WebApi.Domain.Persistence
{
    public interface IRepository<TEntityType>
    {
        Task<IEnumerable<TEntityType>> FindAsync(CancellationToken cancellationToken = new CancellationToken());
        Task<long> CountAsync(CancellationToken cancellationToken = new CancellationToken());
        Task<TEntityType> GetAsync(Guid id, CancellationToken cancellationToken = new CancellationToken());
        Task InsertAsync(TEntityType document, CancellationToken cancellationToken = new CancellationToken());
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = new CancellationToken());
    }
}