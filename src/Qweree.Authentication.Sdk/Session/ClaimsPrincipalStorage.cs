using System;
using System.Security.Claims;

namespace Qweree.Authentication.Sdk.Session;

public class ClaimsPrincipalStorage : ISessionStorage
{
    private readonly Identity _identity;
    public ClaimsPrincipalStorage(ClaimsPrincipal claimsPrincipal)
    {
        ClaimsPrincipal = claimsPrincipal;
        _identity = IdentityMapper.ToIdentity(claimsPrincipal);
    }
    public ClaimsPrincipal ClaimsPrincipal { get; }
    public IdentityUser? CurrentUser => _identity.User;
    public IdentityClient CurrentClient => _identity.Client;
    public Guid Id => CurrentUser?.Id ?? CurrentClient.Id;
    public bool IsAnonymous => Id == Guid.Empty;
}