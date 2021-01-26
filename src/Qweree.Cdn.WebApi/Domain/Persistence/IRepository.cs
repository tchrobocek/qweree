using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Qweree.Cdn.WebApi.Domain.Persistence
{
    public interface IRepository<TEntityType>
    {
        Task<IEnumerable<TEntityType>> FindAsync(CancellationToken cancellationToken = new());
        Task<long> CountAsync(CancellationToken cancellationToken = new());
        Task<TEntityType> GetAsync(Guid id, CancellationToken cancellationToken = new());
        Task InsertAsync(TEntityType document, CancellationToken cancellationToken = new());
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = new());
    }
}