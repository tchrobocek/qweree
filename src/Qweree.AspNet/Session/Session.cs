using System.Collections.Generic;
using System.Collections.Immutable;

namespace Qweree.AspNet.Session
{
    public class Session
    {
        public Session(string username, IEnumerable<string> roles)
        {
            Username = username;
            Roles = roles.ToImmutableArray();
        }

        public string Username { get; }
        public ImmutableArray<string> Roles { get; }
    }
}