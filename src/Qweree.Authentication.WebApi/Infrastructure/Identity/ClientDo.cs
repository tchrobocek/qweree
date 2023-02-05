using System;

namespace Qweree.Authentication.WebApi.Infrastructure.Identity;

public class ClientDo
{
    public Guid? Id { get; set; }
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
    public string? ApplicationName { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public Guid[]? Roles { get; set; }
    public AccessDefinitionDo[]? AccessDefinitions { get; set; }
    public Guid? OwnerId { get; set; }
    public string? Origin { get; set; }
}