using System;
using System.Collections.Immutable;
using System.Linq;
using Qweree.Authentication.AdminSdk.Identity.Users.UserRegister;

namespace Qweree.Authentication.WebApi.Infrastructure.Identity.UserRegister;

public static class UserInvitationMapper
{
    public static UserInvitationDescriptorDo ToDo(UserInvitationDescriptor userInvitationDescriptor)
    {
        return new UserInvitationDescriptorDo
        {
            Id = userInvitationDescriptor.Id,
            Username = userInvitationDescriptor.Username,
            Roles = (userInvitationDescriptor.Roles ?? ImmutableArray<Guid>.Empty).ToArray(),
            ContactEmail = userInvitationDescriptor.ContactEmail,
            FullName = userInvitationDescriptor.FullName,
            CreatedAt = userInvitationDescriptor.CreatedAt,
            ModifiedAt = userInvitationDescriptor.ModifiedAt,
            ExpiresAt = userInvitationDescriptor.ExpiresAt
        };
    }

    public static UserInvitationDescriptor FromDo(UserInvitationDescriptorDo userInvitationDescriptorDo)
    {
        return new UserInvitationDescriptor(userInvitationDescriptorDo.Id ?? Guid.Empty, userInvitationDescriptorDo.Username,
            userInvitationDescriptorDo.FullName, userInvitationDescriptorDo.ContactEmail, userInvitationDescriptorDo.Roles?.ToImmutableArray() ?? ImmutableArray<Guid>.Empty,
            userInvitationDescriptorDo.ExpiresAt ?? DateTime.MinValue, userInvitationDescriptorDo.CreatedAt ?? DateTime.MinValue, userInvitationDescriptorDo.ModifiedAt ?? DateTime.MinValue);
    }
}