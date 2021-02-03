using Qweree.Authentication.WebApi.Domain.Security;

namespace Qweree.Authentication.WebApi.Infrastructure.Security
{
    public class BCryptPasswordEncoder : IPasswordEncoder
    {
        public string EncodePassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string hash, string raw)
        {
            return BCrypt.Net.BCrypt.Verify(raw, hash);
        }
    }
}