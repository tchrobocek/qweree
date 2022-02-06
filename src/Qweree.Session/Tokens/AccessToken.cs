namespace Qweree.Session.Tokens;

public class AccessToken
{
    public AccessToken(Identity identity, DateTime issuedAt, DateTime expiresAt)
    {
        Identity = identity;
        IssuedAt = issuedAt;
        ExpiresAt = expiresAt;
    }

    public Identity Identity { get; }
    public DateTime IssuedAt { get; }
    public DateTime ExpiresAt { get; }
}