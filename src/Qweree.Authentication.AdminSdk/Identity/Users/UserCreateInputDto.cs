namespace Qweree.Authentication.AdminSdk.Identity.Users
{
    public class UserCreateInputDto
    {
        public string? Username { get; set; }
        public string? ContactEmail { get; set; }
        public string? FullName { get; set; }
        public string? Password { get; set; }
        public string[]? Roles { get; set; }
    }
}