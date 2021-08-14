namespace Qweree.Authentication.Sdk.Account
{
    public class ChangeMyPasswordInputDto
    {
        public string? OldPassword { get; set; } = null;
        public string? NewPassword { get; set; } = null;
    }
}