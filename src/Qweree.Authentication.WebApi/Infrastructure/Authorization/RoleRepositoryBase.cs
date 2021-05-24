using System;
using Qweree.Authentication.WebApi.Domain.Authorization;
using Qweree.Mongo;

namespace Qweree.Authentication.WebApi.Infrastructure.Authorization
{
    public class ClientRoleRepository : MongoRepositoryBase<ClientRole, ClientRoleDo>, IClientRoleRepository
    {
        public ClientRoleRepository(MongoContext context) : base("client_roles", context)
        {
        }

        protected override Func<ClientRole, ClientRoleDo> ToDocument => ClientRoleMapper.ToDo;

        protected override Func<ClientRoleDo, ClientRole> FromDocument => ClientRoleMapper.FromDo;
    }

    public class UserRoleRepository : MongoRepositoryBase<UserRole, UserRoleDo>, IUserRoleRepository
    {
        public UserRoleRepository(MongoContext context) : base("user_roles", context)
        {
        }

        protected override Func<UserRole, UserRoleDo> ToDocument => UserRoleMapper.ToDo;

        protected override Func<UserRoleDo, UserRole> FromDocument => UserRoleMapper.FromDo;
    }
}