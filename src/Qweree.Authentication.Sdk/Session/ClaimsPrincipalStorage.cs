using System;
using System.Linq;
using System.Security.Claims;

namespace Qweree.Authentication.Sdk.Session;

public class ClaimsPrincipalStorage : ISessionStorage
{
    private readonly Identity _identity;
    public ClaimsPrincipalStorage(ClaimsPrincipal claimsPrincipal)
    {
        ClaimsPrincipal = claimsPrincipal;
        _identity = IdentityMapper.ToIdentity(claimsPrincipal);
        var sid = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "sid");
        Guid.TryParse(sid?.Value, out var session);
        SessionId = session;
    }
    public ClaimsPrincipal ClaimsPrincipal { get; }
    public Identity Identity => _identity;
    public IdentityUser? CurrentUser => _identity.User;
    public IdentityClient CurrentClient => _identity.Client!;
    public Guid UserId => (Guid)(CurrentUser?.Id ?? CurrentClient.Id)!;
    public bool IsAnonymous => UserId == Guid.Empty;
    public Guid SessionId { get; }
}
