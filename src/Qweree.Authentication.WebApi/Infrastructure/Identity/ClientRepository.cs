using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Authentication.WebApi.Domain.Identity;
using Qweree.Mongo;
using Qweree.Mongo.Exception;

namespace Qweree.Authentication.WebApi.Infrastructure.Identity
{
    public class ClientRepository : MongoRepositoryBase<Client, ClientDo>, IClientRepository
    {
        public ClientRepository(MongoContext context) : base("clients", context)
        {
        }

        protected override Func<Client, ClientDo> ToDocument => ClientMapper.ToDo;
        protected override Func<ClientDo, Client> FromDocument => ClientMapper.FromDo;
        public async Task<Client> GetByClientIdAsync(string clientId, CancellationToken cancellationToken)
        {
            var client = (await FindAsync($@"{{""ClientId"": ""{clientId}""}}", 0, 1, cancellationToken))
                .FirstOrDefault();

            if (client == null)
                throw new DocumentNotFoundException(@$"Client ""{clientId}"" was not found.");

            return client;
        }
    }
}