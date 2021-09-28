using System;
using Qweree.Authentication.WebApi.Domain.Identity;

namespace Qweree.Authentication.WebApi.Test.Fixture.Factories
{
    public static class UserFactory
    {
        public static User CreateDefault(string username = "user")
        {
            return new(Guid.NewGuid(), username, "User Userov", "user@example.com",
                "pwd", Array.Empty<Guid>(), DateTime.UtcNow, DateTime.UtcNow);
        }

        public static User CreateAdmin()
        {
            return new(Guid.NewGuid(), "user", "User Userov", "user@example.com",
                "pwd", new[] {Guid.Parse("d0a77eeb-972e-4337-a62e-493b3e59f214"),
                    Guid.Parse("c990cc7b-7415-4836-8468-b48c67dd9e45"),
                    Guid.Parse("66a81b6b-fd91-4338-8ca3-e4aed14dd868"),
                    Guid.Parse("20352123-e4c1-4e37-affa-e136d9a66d02"),
                    Guid.Parse("e2ec78fd-c5a0-4b37-b57a-a0a3363e1798"),
                    Guid.Parse("729fde13-52a2-4a82-befa-a4e6666924a6"),
                    Guid.Parse("2b0e761c-cce2-4609-9ab6-94980fbc639e"),
                    Guid.Parse("f7f99a7b-6890-4bf2-b39b-14444585a712"),
                    Guid.Parse("905b39a6-a4bd-480a-b83d-397d1add5569"),
                    Guid.Parse("a1272efa-b687-4fda-a999-11b4b0acd414"),
                    Guid.Parse("98d7d3a1-bedd-4ee1-a633-a4217f5414ee"),
                    Guid.Parse("145e1674-691a-4bd5-956b-4154bc4264da"),
                    Guid.Parse("d98049ab-977e-42ef-bba6-05c16184d054")}, DateTime.UtcNow, DateTime.UtcNow);
        }
    }
}