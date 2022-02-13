using System;

namespace Qweree.Authentication.Sdk.Session;

public interface ISessionStorage
{
    IdentityUser? CurrentUser { get; }
    IdentityClient CurrentClient { get; }
    Guid Id { get; }
    bool IsAnonymous { get; }
}