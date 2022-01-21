using System.Threading;
using System.Threading.Tasks;
using Qweree.Authentication.WebApi.Domain.Persistence;

namespace Qweree.Authentication.WebApi.Domain.Identity;

public interface IUserRepository : IRepository<User>
{
    Task<User> GetByUsernameAsync(string username, CancellationToken cancellationToken = new());
}