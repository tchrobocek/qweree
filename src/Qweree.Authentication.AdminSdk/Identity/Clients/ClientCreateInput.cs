using System;

namespace Qweree.Authentication.AdminSdk.Identity.Clients;

public class ClientCreateInput
{
    public Guid? Id { get; set; }
    public string? ClientId { get; set; }
    public string? ApplicationName { get; set; }
    public string? Origin { get; set; }
    public Guid? OwnerId { get; set; }
    public Guid[]? UserRoles { get; set; }
}