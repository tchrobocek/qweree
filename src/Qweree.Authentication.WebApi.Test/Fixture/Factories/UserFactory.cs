using System;
using Qweree.Authentication.WebApi.Domain.Identity;

namespace Qweree.Authentication.WebApi.Test.Fixture.Factories
{
    public static class UserFactory
    {
        public const string Password = "pwd";
        public static User CreateDefault()
        {
            return new User(Guid.NewGuid(), "user", "User Userov", "user@example.com", BCrypt.Net.BCrypt.HashPassword(Password), Array.Empty<string>(), DateTime.UtcNow, DateTime.UtcNow);
        }

        public static User CreateAdmin()
        {
            return new User(Guid.NewGuid(), "user", "User Userov", "user@example.com", BCrypt.Net.BCrypt.HashPassword(Password), new[] {"UserUpdate"}, DateTime.UtcNow, DateTime.UtcNow);
        }
    }
}