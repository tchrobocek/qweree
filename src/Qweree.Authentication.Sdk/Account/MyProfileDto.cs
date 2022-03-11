using System;
using Qweree.Authentication.Sdk.Users;

namespace Qweree.Authentication.Sdk.Account;

public class MyProfileDto
{
    public Guid? Id { get; set; }
    public string? Username { get; set; }
    public string? ContactEmail { get; set; }
    public UserPropertyDto[]? Properties { get; set; }
}