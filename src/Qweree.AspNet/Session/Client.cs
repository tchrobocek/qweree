using System;

namespace Qweree.AspNet.Session;

public class Client
{
    public Client(Guid clientId)
    {
        ClientId = clientId;
    }

    public Guid ClientId { get; }
}