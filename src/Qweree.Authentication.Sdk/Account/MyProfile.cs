using System;
using Qweree.Authentication.Sdk.Users;

namespace Qweree.Authentication.Sdk.Account;

public class MyProfile
{
    public Guid? Id { get; set; }
    public string? Username { get; set; }
    public string? ContactEmail { get; set; }
    public UserProperty[]? Properties { get; set; }
}