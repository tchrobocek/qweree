using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Mongo;

namespace Qweree.Authentication.WebApi.Domain.Persistence;

public interface IRepository<TEntityType>
{
    Task<IEnumerable<TEntityType>> FindAsync(CancellationToken cancellationToken = new());
    Task<Pagination<TEntityType>> PaginateAsync(int skip, int take, Dictionary<string, int> sort,
        CancellationToken cancellationToken = new());
    Task<long> CountAsync(CancellationToken cancellationToken = new());
    Task<TEntityType> GetAsync(Guid id, CancellationToken cancellationToken = new());
    Task<IEnumerable<TEntityType>> GetAsync(IEnumerable<Guid> ids, bool strict = false, CancellationToken cancellationToken = new());
    Task InsertAsync(TEntityType document, CancellationToken cancellationToken = new());
    Task DeleteOneAsync(Guid id, CancellationToken cancellationToken = new());
    Task ReplaceAsync(string id, TEntityType document, CancellationToken cancellationToken = new());
}