using Qweree.Authentication.WebApi.Domain.Persistence;

namespace Qweree.Authentication.WebApi.Domain.Authorization.Roles
{
    public interface IUserRoleRepository : IRepository<UserRole>
    {

    }

    public interface IClientRoleRepository : IRepository<ClientRole>
    {

    }
}