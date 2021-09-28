using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Authentication.WebApi.Domain.Persistence;

namespace Qweree.Authentication.WebApi.Domain.Authorization.Roles
{
    public interface IClientRoleRepository : IRepository<ClientRole>
    {
        Task<ClientRole?> FindByKey(string key, CancellationToken cancellationToken = new());
        Task<IEnumerable<ClientRole>> FindParentRolesAsync(Guid id, CancellationToken cancellationToken = new());
    }
}