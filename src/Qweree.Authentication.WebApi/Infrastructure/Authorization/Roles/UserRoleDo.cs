using System;

namespace Qweree.Authentication.WebApi.Infrastructure.Authorization.Roles;

public class RoleDo
{
    public Guid? Id { get; set; }
    public string? Key { get; set; }
    public string? Label { get; set; }
    public string? Description { get; set; }
    public Guid[]? Items { get; set; }
    public bool? IsGroup { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}