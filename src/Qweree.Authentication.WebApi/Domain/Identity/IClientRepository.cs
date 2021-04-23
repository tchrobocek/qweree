using System.Threading;
using System.Threading.Tasks;
using Qweree.Authentication.WebApi.Domain.Persistence;

namespace Qweree.Authentication.WebApi.Domain.Identity
{
    public interface IClientRepository : IRepository<Client>
    {
        Task<Client> GetByClientIdAsync(string clientId, CancellationToken cancellationToken);
    }
}