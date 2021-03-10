using System;
using Qweree.Authentication.AdminSdk.Identity.Clients;

namespace Qweree.Authentication.AdminSdk.Test.TestUtils
{
    public static class ClientFactory
    {
        public static Client CreateValid()
        {
            return new(Guid.NewGuid(), "client", "application", "http://yomom.com", UserFactory.CreateValid(),
                DateTime.UtcNow, DateTime.UtcNow);
        }

        public static CreatedClient CreateValidCreatedClient()
        {
            return new(Guid.NewGuid(), "client", "application", "http://yomom.com", UserFactory.CreateValid(),
                DateTime.UtcNow, DateTime.UtcNow);
        }
    }
}