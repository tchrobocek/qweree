namespace Qweree.Authentication.WebApi.Domain.Security
{
    public interface IPasswordEncoder
    {
        string EncodePassword(string password);
        bool VerifyPassword(string hash, string raw);
    }
}