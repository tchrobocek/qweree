using System;
using Qweree.Authentication.Sdk.Identity;

namespace Qweree.Authentication.Sdk.Account.MyAccount;

public class MyProfile
{
    public Guid? Id { get; set; }
    public string? Username { get; set; }
    public string? ContactEmail { get; set; }
    public UserProperty[]? Properties { get; set; }
}