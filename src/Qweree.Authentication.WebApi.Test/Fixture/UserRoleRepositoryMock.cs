using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Authentication.WebApi.Domain.Authorization.Roles;
using Qweree.Mongo;

namespace Qweree.Authentication.WebApi.Test.Fixture;

public class UserRoleRepositoryMock : IUserRoleRepository
{
    public static IEnumerable<UserRole> GetRoles()
    {
        yield return new UserRole(Guid.Parse("d0a77eeb-972e-4337-a62e-493b3e59f214"), "AUTH_USERS_CREATE", "", "",
            ImmutableArray<Guid>.Empty, false, DateTime.MinValue, DateTime.MinValue);
        yield return new UserRole(Guid.Parse("c990cc7b-7415-4836-8468-b48c67dd9e45"), "AUTH_USERS_READ", "", "",
            ImmutableArray<Guid>.Empty, false, DateTime.MinValue, DateTime.MinValue);
        yield return new UserRole(Guid.Parse("66a81b6b-fd91-4338-8ca3-e4aed14dd868"), "AUTH_USERS_DELETE", "", "",
            ImmutableArray<Guid>.Empty, false, DateTime.MinValue, DateTime.MinValue);
        yield return new UserRole(Guid.Parse("20352123-e4c1-4e37-affa-e136d9a66d02"),
            "AUTH_USERS_READ_PERSONAL_DETAIL", "", "", ImmutableArray<Guid>.Empty, false, DateTime.MinValue,
            DateTime.MinValue);
        yield return new UserRole(Guid.Parse("e2ec78fd-c5a0-4b37-b57a-a0a3363e1798"), "AUTH_CLIENTS_CREATE", "", "",
            ImmutableArray<Guid>.Empty, false, DateTime.MinValue, DateTime.MinValue);
        yield return new UserRole(Guid.Parse("729fde13-52a2-4a82-befa-a4e6666924a6"), "AUTH_CLIENTS_READ", "", "",
            ImmutableArray<Guid>.Empty, false, DateTime.MinValue, DateTime.MinValue);
        yield return new UserRole(Guid.Parse("2b0e761c-cce2-4609-9ab6-94980fbc639e"), "AUTH_CLIENTS_DELETE", "", "",
            ImmutableArray<Guid>.Empty, false, DateTime.MinValue, DateTime.MinValue);
        yield return new UserRole(Guid.Parse("f7f99a7b-6890-4bf2-b39b-14444585a712"), "AUTH_ROLES_CREATE", "", "",
            ImmutableArray<Guid>.Empty, false, DateTime.MinValue, DateTime.MinValue);
        yield return new UserRole(Guid.Parse("905b39a6-a4bd-480a-b83d-397d1add5569"), "AUTH_ROLES_READ", "", "",
            ImmutableArray<Guid>.Empty, false, DateTime.MinValue, DateTime.MinValue);
        yield return new UserRole(Guid.Parse("a1272efa-b687-4fda-a999-11b4b0acd414"), "AUTH_ROLES_DELETE", "", "",
            ImmutableArray<Guid>.Empty, false, DateTime.MinValue, DateTime.MinValue);
        yield return new UserRole(Guid.Parse("98d7d3a1-bedd-4ee1-a633-a4217f5414ee"), "AUTH_ROLES_MODIFY", "", "",
            ImmutableArray<Guid>.Empty, false, DateTime.MinValue, DateTime.MinValue);
    }

    public Task<IEnumerable<UserRole>> FindAsync(CancellationToken cancellationToken = new())
    {
        throw new NotImplementedException();
    }

    public Task<Pagination<UserRole>> PaginateAsync(int skip, int take, Dictionary<string, int> sort,
        CancellationToken cancellationToken = new())
    {
        throw new NotImplementedException();
    }

    public Task<long> CountAsync(CancellationToken cancellationToken = new())
    {
        throw new NotImplementedException();
    }

    public Task<UserRole> GetAsync(Guid id, CancellationToken cancellationToken = new())
    {
        return Task.FromResult(GetRoles().First(r => r.Id == id));
    }

    public Task InsertAsync(UserRole document, CancellationToken cancellationToken = new())
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = new())
    {
        throw new NotImplementedException();
    }

    public Task ReplaceAsync(string id, UserRole document,
        CancellationToken cancellationToken = new())
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<UserRole>> FindParentRolesAsync(Guid id, CancellationToken cancellationToken = new())
    {
        throw new NotImplementedException();
    }
}