using System;
using System.Collections.Immutable;
using Qweree.Authentication.AdminSdk.Authorization.Roles;
using Qweree.Authentication.AdminSdk.Identity.Clients;

namespace Qweree.Authentication.AdminSdk.Test.TestUtils;

public static class ClientFactory
{
    public static Client CreateValid()
    {
        return new Client(Guid.NewGuid(), "client", "application", "http://yomom.com", UserFactory.CreateValid(), ImmutableArray<Role>.Empty,ImmutableArray<Role>.Empty,
            ImmutableArray<Role>.Empty, ImmutableArray<Role>.Empty,
            DateTime.UtcNow, DateTime.UtcNow);
    }

    public static CreatedClient CreateValidCreatedClient()
    {
        return new CreatedClient(Guid.NewGuid(), "client", "application", "http://yomom.com", "localhost", UserFactory.CreateValid(),
            DateTime.UtcNow, DateTime.UtcNow, ImmutableArray<Role>.Empty);
    }
}