using System;
using System.Collections.Immutable;
using Qweree.Authentication.AdminSdk.Identity.Users;

namespace Qweree.Authentication.AdminSdk.Test.TestUtils
{
    public static class UserFactory
    {
        public static User CreateValid()
        {
            return new(Guid.NewGuid(), "user", "User User", "user@example.com", new[] {"role1", "role2"}.ToImmutableArray(),
                DateTime.UtcNow, DateTime.UtcNow);
        }
    }
}