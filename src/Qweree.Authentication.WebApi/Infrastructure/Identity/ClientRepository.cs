using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Authentication.WebApi.Domain.Identity;
using Qweree.Authentication.WebApi.Infrastructure.Validations;
using Qweree.Mongo;
using Qweree.Mongo.Exception;

namespace Qweree.Authentication.WebApi.Infrastructure.Identity;

public class ClientRepository : MongoRepositoryBase<Client, ClientDo>, IClientRepository, IUniqueConstraintValidatorRepository
{
    public ClientRepository(MongoContext context) : base("clients", context)
    {
    }

    protected override Func<Client, ClientDo> ToDocument => ClientMapper.ToClientDo;
    protected override Func<ClientDo, Client> FromDocument => ClientMapper.ToClient;
    public async Task<Client> GetByClientIdAsync(string clientId, CancellationToken cancellationToken)
    {
        var client = (await FindAsync($@"{{""ClientId"": ""{clientId}""}}", 0, 1, cancellationToken))
            .FirstOrDefault();

        if (client == null)
            throw new DocumentNotFoundException(@$"Client ""{clientId}"" was not found.");

        return client;
    }

    public async Task<bool> IsExistingAsync(string field, string value, CancellationToken cancellationToken = new())
    {
        var client = (await FindAsync($@"{{""{field}"": ""{value}""}}", 0, 1, cancellationToken))
            .FirstOrDefault();

        return client != null;
    }
}