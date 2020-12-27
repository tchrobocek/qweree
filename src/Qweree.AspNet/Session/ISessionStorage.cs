namespace Qweree.AspNet.Session
{
    public interface ISessionStorage
    {
        User CurrentUser { get; }
    }
}