using System;
using System.Collections.Immutable;
using Qweree.Authentication.WebApi.Domain.Identity;

namespace Qweree.Authentication.WebApi.Test.Fixture.Factories;

public class ClientFactory
{
    public static Client CreateDefault(Guid ownerId, string clientId = "client")
    {
        return new Client(Guid.NewGuid(), clientId, "secret", "application",
            ImmutableArray<IAccessDefinition>.Empty, DateTime.UtcNow, DateTime.UtcNow, ownerId, "http://yomom.com");
    }
}