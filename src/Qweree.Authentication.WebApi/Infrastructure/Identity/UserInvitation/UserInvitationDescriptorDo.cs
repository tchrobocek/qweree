using System;

namespace Qweree.Authentication.WebApi.Infrastructure.Identity.UserInvitation;

public class UserInvitationDescriptorDo
{
    public Guid? Id { get; set; }
    public string? Username { get; set; }
    public string? FullName { get; set; }
    public string? ContactEmail { get; set; }
    public Guid[]? Roles { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}