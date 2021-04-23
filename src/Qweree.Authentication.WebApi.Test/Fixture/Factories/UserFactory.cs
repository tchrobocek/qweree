using System;
using Qweree.Authentication.WebApi.Domain.Identity;

namespace Qweree.Authentication.WebApi.Test.Fixture.Factories
{
    public static class UserFactory
    {
        public static User CreateDefault(string username = "user")
        {
            return new(Guid.NewGuid(), username, "User Userov", "user@example.com",
                "pwd", Array.Empty<string>(), DateTime.UtcNow, DateTime.UtcNow);
        }

        public static User CreateAdmin()
        {
            return new(Guid.NewGuid(), "user", "User Userov", "user@example.com",
                "pwd", new[] {"AUTH_USERS_CREATE", "AUTH_USERS_READ", "AUTH_USERS_DELETE", "AUTH_USERS_READ_PERSONAL_DETAIL", "AUTH_CLIENTS_CREATE", "AUTH_CLIENTS_READ", "AUTH_CLIENTS_DELETE", }, DateTime.UtcNow, DateTime.UtcNow);
        }
    }
}