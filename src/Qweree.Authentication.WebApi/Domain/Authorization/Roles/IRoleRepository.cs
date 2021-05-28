using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Authentication.WebApi.Domain.Persistence;

namespace Qweree.Authentication.WebApi.Domain.Authorization.Roles
{
    public interface IUserRoleRepository : IRepository<UserRole>
    {
        Task<IEnumerable<UserRole>> FindParentRolesAsync(Guid id, CancellationToken cancellationToken = new());
    }

    public interface IClientRoleRepository : IRepository<ClientRole>
    {
        Task<IEnumerable<ClientRole>> FindParentRolesAsync(Guid id, CancellationToken cancellationToken = new());
    }
}