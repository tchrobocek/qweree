using System;

namespace Qweree.Authentication.Sdk.Identity;

public class Client
{
    public Guid? Id { get; set; }
    public string? ClientId { get; set; }
    public string? ApplicationName { get; set; }
}