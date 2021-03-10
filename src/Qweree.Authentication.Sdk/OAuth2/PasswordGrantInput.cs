namespace Qweree.Authentication.Sdk.OAuth2
{
    public class PasswordGrantInput
    {
        public PasswordGrantInput(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public string Username { get; }
        public string Password { get; }
    }
}