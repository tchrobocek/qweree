using System;
using System.Collections.Immutable;
using System.Linq;
using Qweree.Authentication.AdminSdk.Identity.Users.UserInvitation;

namespace Qweree.Authentication.WebApi.Domain.Identity.UserInvitation;

public static class UserInvitationDescriptorMapper
{
    public static UserInvitationDescriptorDto ToDto(UserInvitationDescriptor userInvitationDescriptor)
    {
        return new UserInvitationDescriptorDto
        {
            Id = userInvitationDescriptor.Id,
            Roles = (userInvitationDescriptor.Roles ?? ImmutableArray<Guid>.Empty).ToArray(),
            Username = userInvitationDescriptor.Username,
            ContactEmail = userInvitationDescriptor.ContactEmail,
            FullName = userInvitationDescriptor.FullName,
            CreatedAt = userInvitationDescriptor.CreatedAt,
            ExpiresAt = userInvitationDescriptor.ExpiresAt,
            ModifiedAt = userInvitationDescriptor.ModifiedAt,
        };
    }
}