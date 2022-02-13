using System;
using System.Collections.Immutable;
using Qweree.Authentication.AdminSdk.Authorization.Roles;
using Qweree.Authentication.AdminSdk.Identity.Users;
using Qweree.Authentication.Sdk.Users;

namespace Qweree.Authentication.AdminSdk.Test.TestUtils;

public static class UserFactory
{
    public static User CreateValid()
    {
        return new User(Guid.NewGuid(), "user", "user@example.com", new UserProperty[] {new("full_name", "Full Name")}.ToImmutableArray(), new[] {new Role(Guid.NewGuid(), "role1", "", ""), new Role(Guid.NewGuid(), "role2", "", "")}.ToImmutableArray(),
            DateTime.UtcNow, DateTime.UtcNow);
    }
}