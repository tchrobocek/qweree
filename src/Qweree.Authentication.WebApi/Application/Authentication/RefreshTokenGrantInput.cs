namespace Qweree.Authentication.WebApi.Application.Authentication
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