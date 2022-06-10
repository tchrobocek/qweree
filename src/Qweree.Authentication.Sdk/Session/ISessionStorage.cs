using System;

namespace Qweree.Authentication.Sdk.Session;

public interface ISessionStorage
{
    IdentityUser? CurrentUser { get; }
    IdentityClient CurrentClient { get; }
    Guid UserId { get; }
    Guid SessionId { get; }
    bool IsAnonymous { get; }
}