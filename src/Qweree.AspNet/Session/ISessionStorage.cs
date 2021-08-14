using System;

namespace Qweree.AspNet.Session
{
    public interface ISessionStorage
    {
        User? CurrentUser { get; }
        Client CurrentClient { get; }
        Guid Id { get; }
    }
}