namespace Qweree.Authentication.Sdk.Account
{
    public class ChangeMyPasswordInput
    {
        public ChangeMyPasswordInput(string oldPassword, string newPassword)
        {
            OldPassword = oldPassword;
            NewPassword = newPassword;
        }

        public string OldPassword { get; }
        public string NewPassword { get; }
    }
}