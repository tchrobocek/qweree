namespace Qweree.Authentication.WebApi.Domain.Authentication
{
    public class RefreshTokenGrantInput
    {
        public RefreshTokenGrantInput(string refreshToken)
        {
            RefreshToken = refreshToken;
        }

        public string RefreshToken { get; }
    }
}