namespace Qweree.Session;

public interface ISessionStorage
{
    IdentityUser? CurrentUser { get; }
    IdentityClient CurrentClient { get; }
    Guid Id { get; }
}