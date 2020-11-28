using System;

namespace Qweree.Authentication.Sdk.Identity
{
    public class User
    {
        public User(Guid id, string username, string fullName)
        {
            Id = id;
            Username = username;
            FullName = fullName;
        }

        public Guid Id { get; }
        public string Username { get; }
        public string FullName { get; }
    }
}