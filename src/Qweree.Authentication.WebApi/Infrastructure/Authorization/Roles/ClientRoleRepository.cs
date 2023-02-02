using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Authentication.WebApi.Domain.Authorization.Roles;
using Qweree.Authentication.WebApi.Infrastructure.Validations;
using Qweree.Mongo;

namespace Qweree.Authentication.WebApi.Infrastructure.Authorization.Roles;

public class ClientRoleRepository : MongoRepositoryBase<ClientRole, ClientRoleDo>, IClientRoleRepository,
    IUniqueConstraintValidatorRepository, IExistsConstraintValidatorRepository
{
    public ClientRoleRepository(MongoContext context) : base("client_roles", context)
    {
    }

    protected override Func<ClientRole, ClientRoleDo> ToDocument => RoleMapper.ToClientRoleDo;

    protected override Func<ClientRoleDo, ClientRole> FromDocument => RoleMapper.ToClientRole;

    public async Task<bool> IsExistingAsync(string field, string value, CancellationToken cancellationToken = new())
    {
        var clientRole = (await FindAsync($@"{{""{field}"": ""{value}""}}", 0, 1, cancellationToken))
            .FirstOrDefault();

        return clientRole != null;
    }

    public async Task<ClientRole?> FindByKey(string key, CancellationToken cancellationToken = new())
    {
        var clientRole = (await FindAsync($@"{{""Key"": ""{key}""}}", 0, 1, cancellationToken))
            .FirstOrDefault();

        return clientRole;
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