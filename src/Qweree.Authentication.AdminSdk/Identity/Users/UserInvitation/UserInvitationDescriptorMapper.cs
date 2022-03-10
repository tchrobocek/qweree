using System;
using System.Collections.Immutable;
using System.Linq;

namespace Qweree.Authentication.AdminSdk.Identity.Users.UserInvitation;

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

    public static UserInvitationDescriptor FromDto(UserInvitationDescriptorDto userInvitationDescriptorDto)
    {
        return new UserInvitationDescriptor(userInvitationDescriptorDto.Id ?? Guid.Empty, userInvitationDescriptorDto.Username,
            userInvitationDescriptorDto.FullName, userInvitationDescriptorDto.ContactEmail,
            userInvitationDescriptorDto.Roles?.ToImmutableArray(), userInvitationDescriptorDto.ExpiresAt ?? DateTime.MinValue, userInvitationDescriptorDto.CreatedAt ?? DateTime.MinValue,
            userInvitationDescriptorDto.ModifiedAt ?? DateTime.MinValue);
    }
}