using System;
using Qweree.Authentication.WebApi.Domain.Identity;
using Qweree.Mongo;

namespace Qweree.Authentication.WebApi.Infrastructure.Identity
{
    public class ClientRepository : MongoRepositoryBase<Client, ClientDo>, IClientRepository
    {
        public ClientRepository(MongoContext context) : base("clients", context)
        {
        }

        protected override Func<Client, ClientDo> ToDocument => ClientMapper.ToDo;
        protected override Func<ClientDo, Client> FromDocument => ClientMapper.FromDo;
    }
}