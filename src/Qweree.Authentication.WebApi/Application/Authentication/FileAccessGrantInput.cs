namespace Qweree.Authentication.WebApi.Application.Authentication
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