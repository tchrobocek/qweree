using System.Security.Claims;

namespace Qweree.Session;

public class ClaimsPrincipalStorage : ISessionStorage
{
    private readonly Identity _identity;
    public ClaimsPrincipalStorage(ClaimsPrincipal claimsPrincipal)
    {
        ClaimsPrincipal = claimsPrincipal;
        _identity = ClaimsPrincipalMapper.CreateIdentity(claimsPrincipal);
    }
    public ClaimsPrincipal ClaimsPrincipal { get; }
    public IdentityUser? CurrentUser => _identity.User;
    public IdentityClient CurrentClient => _identity.Client;
    public Guid Id => CurrentUser?.Id ?? CurrentClient.Id;
    public bool IsAnonymous => Id == Guid.Empty;
}