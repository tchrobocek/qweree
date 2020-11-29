using Qweree.Authentication.Sdk.Identity;

namespace Qweree.AspNet.Session
{
    public interface ISessionStorage
    {
        User CurrentUser { get; }
    }
}