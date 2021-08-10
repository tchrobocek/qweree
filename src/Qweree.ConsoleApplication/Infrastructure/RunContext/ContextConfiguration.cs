namespace Qweree.ConsoleApplication.Infrastructure.RunContext
{
    public class ContextConfiguration
    {
        public ContextConfiguration(string username, string refreshToken)
        {
            Username = username;
            RefreshToken = refreshToken;
        }

        public string Username { get; }
        public string RefreshToken { get; }
    }
    public class ContextConfigurationDo
    {
        public string? Username { get; set; }
        public string? RefreshToken { get; set; }
    }
}