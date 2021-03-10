using System;
using Qweree.Authentication.WebApi.Domain.Identity;

namespace Qweree.Authentication.WebApi.Test.Fixture.Factories
{
    public class ClientFactory
    {
        public static Client CreateDefault(Guid ownerId, string clientId = "client")
        {
            return new(Guid.NewGuid(), clientId, "secret", "application",
                DateTime.UtcNow, DateTime.UtcNow, ownerId, "http://yomom.com");
        }
    }
}