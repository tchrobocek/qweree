using System;
using System.Collections.Immutable;
using Qweree.Authentication.WebApi.Domain.Identity;

namespace Qweree.Authentication.WebApi.Test.Fixture.Factories
{
    public class ClientFactory
    {
        public static Client CreateDefault(Guid ownerId, string clientId = "client")
        {
            return new(Guid.NewGuid(), clientId, "secret", "application", new[] {Guid.Parse("f946f6cd-5d17-4dc5-b383-af0201a8b431")}.ToImmutableArray(), ImmutableArray<Guid>.Empty,
                DateTime.UtcNow, DateTime.UtcNow, ownerId, "http://yomom.com");
        }
    }
}