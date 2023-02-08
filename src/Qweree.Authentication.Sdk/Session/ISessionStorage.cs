using System;

namespace Qweree.Authentication.Sdk.Session;

public interface ISessionStorage
{
    IdentityUser? CurrentUser { get; }
    IdentityClient CurrentClient { get; }
    Identity Identity { get; }
    Guid UserId { get; }
    Guid SessionId { get; }
    bool IsAnonymous { get; }
}