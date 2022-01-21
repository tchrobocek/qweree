using System;
using System.Collections.Immutable;
using Qweree.Authentication.WebApi.Domain.Identity;

namespace Qweree.Authentication.WebApi.Test.Fixture.Factories;

public class ClientFactory
{
    public static Client CreateDefault(Guid ownerId, string clientId = "client")
    {
        return new Client(Guid.NewGuid(), clientId, "secret", "application", new[] {Guid.Parse("f946f6cd-5d17-4dc5-b383-af0201a8b431"), Guid.Parse("2ec76030-c18d-450c-9a0e-5ff9efb1721d"), Guid.Parse("92024488-4c7d-42a9-8fc5-19fc73853c8e")}.ToImmutableArray(), ImmutableArray<Guid>.Empty,
            DateTime.UtcNow, DateTime.UtcNow, ownerId, "http://yomom.com");
    }
}