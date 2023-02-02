using System;
using System.Collections.Immutable;
using System.Linq;
using SdkUserInvitation = Qweree.Authentication.AdminSdk.Identity.Users.UserInvitation.UserInvitation;

namespace Qweree.Authentication.WebApi.Domain.Identity.UserInvitation;

public static class UserInvitationDescriptorMapper
{
    public static SdkUserInvitation ToUserInvitation(UserInvitationDescriptor userInvitationDescriptor)
    {
        return new SdkUserInvitation
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