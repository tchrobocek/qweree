namespace Qweree.Authentication.Sdk
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