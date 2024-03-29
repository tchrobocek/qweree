using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Authentication.WebApi.Domain.Persistence;

namespace Qweree.Authentication.WebApi.Domain.Authorization.Roles;

public interface IRoleRepository : IRepository<Role>
{
    Task<IEnumerable<Role>> FindParentRolesAsync(Guid id, CancellationToken cancellationToken = new());
    Task<Role?> FindByKey(string key, CancellationToken cancellationToken = new());
}