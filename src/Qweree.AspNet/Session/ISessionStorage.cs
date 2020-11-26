namespace Qweree.AspNet.Session
{
    public interface ISessionStorage
    {
        Session CurrentSession { get; }
    }
}