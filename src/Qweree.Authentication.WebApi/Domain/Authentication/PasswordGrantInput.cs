namespace Qweree.Authentication.WebApi.Domain.Authentication;

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