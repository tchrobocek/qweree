using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Mongo;

namespace Qweree.Authentication.WebApi.Domain.Persistence
{
    public interface IRepository<TEntityType>
    {
        Task<IEnumerable<TEntityType>> FindAsync(CancellationToken cancellationToken = new CancellationToken());
        Task<Pagination<TEntityType>> PaginateAsync(int take, int skip, Dictionary<string, int> sort, CancellationToken cancellationToken = new CancellationToken());
        Task<long> CountAsync(CancellationToken cancellationToken = new CancellationToken());
        Task<TEntityType> GetAsync(Guid id, CancellationToken cancellationToken = new CancellationToken());
        Task InsertAsync(TEntityType document, CancellationToken cancellationToken = new CancellationToken());
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = new CancellationToken());
    }
}