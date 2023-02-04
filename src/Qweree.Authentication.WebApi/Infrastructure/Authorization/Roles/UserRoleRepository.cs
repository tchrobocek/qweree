using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Authentication.WebApi.Domain.Authorization.Roles;
using Qweree.Authentication.WebApi.Infrastructure.Validations;
using Qweree.Mongo;

namespace Qweree.Authentication.WebApi.Infrastructure.Authorization.Roles;

public class RoleRepository : MongoRepositoryBase<Role, RoleDo>, IRoleRepository, IUniqueConstraintValidatorRepository, IExistsConstraintValidatorRepository
{
    public RoleRepository(MongoContext context) : base("roles", context)
    {
    }

    protected override Func<Role, RoleDo> ToDocument => RoleMapper.ToRoleDo;

    protected override Func<RoleDo, Role> FromDocument => RoleMapper.ToRole;

    public async Task<bool> IsExistingAsync(string field, string value, CancellationToken cancellationToken = new())
    {
        var role = (await FindAsync($@"{{""{field}"": ""{value}""}}", 0, 1, cancellationToken))
            .FirstOrDefault();

        return role is not null;
    }

    public async Task<IEnumerable<Role>> FindParentRolesAsync(Guid id, CancellationToken cancellationToken = new())
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

    public async Task<Role?> FindByKey(string key, CancellationToken cancellationToken = new())
    {
        var role = (await FindAsync($@"{{""Key"": ""{key}""}}", 0, 1, cancellationToken))
            .FirstOrDefault();

        return role;
    }
}