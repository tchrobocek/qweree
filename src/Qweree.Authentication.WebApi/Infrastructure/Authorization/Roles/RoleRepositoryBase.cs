using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Authentication.WebApi.Domain.Authorization.Roles;
using Qweree.Authentication.WebApi.Infrastructure.Validations;
using Qweree.Mongo;

namespace Qweree.Authentication.WebApi.Infrastructure.Authorization.Roles
{
    public class ClientRoleRepository : MongoRepositoryBase<ClientRole, ClientRoleDo>, IClientRoleRepository, IUniqueConstraintValidatorRepository, IExistsConstraintValidatorRepository
    {
        public ClientRoleRepository(MongoContext context) : base("client_roles", context)
        {
        }

        protected override Func<ClientRole, ClientRoleDo> ToDocument => ClientRoleMapper.ToDo;

        protected override Func<ClientRoleDo, ClientRole> FromDocument => ClientRoleMapper.FromDo;

        public async Task<bool> IsExistingAsync(string field, string value, CancellationToken cancellationToken = new())
        {
            var clientRole = (await FindAsync($@"{{""{field}"": ""{value}""}}", 0, 1, cancellationToken))
                .FirstOrDefault();

            return clientRole != null;
        }

        public async Task<IEnumerable<ClientRole>> FindParentRolesAsync(Guid id, CancellationToken cancellationToken = new())
        {
            return await FindAsync($@"{{""Items"": UUID(""{id}"")}}", 0, 1, cancellationToken);
        }


        public async Task<bool> IsExistingAsync(string value, CancellationToken cancellationToken = new())
        {
            if (!Guid.TryParse(value, out var guid))
                return false;

            try
            {
                await GetAsync(guid, cancellationToken);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    public class UserRoleRepository : MongoRepositoryBase<UserRole, UserRoleDo>, IUserRoleRepository, IUniqueConstraintValidatorRepository, IExistsConstraintValidatorRepository
    {
        public UserRoleRepository(MongoContext context) : base("user_roles", context)
        {
        }

        protected override Func<UserRole, UserRoleDo> ToDocument => UserRoleMapper.ToDo;

        protected override Func<UserRoleDo, UserRole> FromDocument => UserRoleMapper.FromDo;

        public async Task<bool> IsExistingAsync(string field, string value, CancellationToken cancellationToken = new())
        {
            var userRole = (await FindAsync($@"{{""{field}"": ""{value}""}}", 0, 1, cancellationToken))
                .FirstOrDefault();

            return userRole != null;
        }

        public async Task<IEnumerable<UserRole>> FindParentRolesAsync(Guid id, CancellationToken cancellationToken = new())
        {
            return await FindAsync($@"{{""Items"": UUID(""{id}"")}}", 0, 1, cancellationToken);
        }

        public async Task<bool> IsExistingAsync(string value, CancellationToken cancellationToken = new())
        {
            if (!Guid.TryParse(value, out var guid))
                return false;

            try
            {
                await GetAsync(guid, cancellationToken);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}