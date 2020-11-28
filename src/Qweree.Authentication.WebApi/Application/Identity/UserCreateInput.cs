using System.Collections.Generic;
using System.Collections.Immutable;

namespace Qweree.Authentication.WebApi.Application.Identity
{
    public class UserCreateInput
    {
        public UserCreateInput(string username, string contactEmail, string fullName, string password, IEnumerable<string> roles)
        {
            Username = username;
            ContactEmail = contactEmail;
            FullName = fullName;
            Password = password;
            Roles = roles.ToImmutableArray();
        }

        public string Username { get; }
        public string ContactEmail { get; }
        public string FullName { get; }
        public string Password { get; }
        public ImmutableArray<string> Roles { get; }
    }
}