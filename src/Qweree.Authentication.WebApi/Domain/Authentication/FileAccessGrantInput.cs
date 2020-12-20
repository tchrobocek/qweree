namespace Qweree.Authentication.WebApi.Domain.Authentication
{
    public class FileAccessGrantInput
    {
        public FileAccessGrantInput(string accessToken)
        {
            AccessToken = accessToken;
        }

        public string AccessToken { get; }
    }
}