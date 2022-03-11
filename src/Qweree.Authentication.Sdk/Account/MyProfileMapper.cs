using System;
using System.Collections.Immutable;
using System.Linq;
using Qweree.Authentication.Sdk.Users;

namespace Qweree.Authentication.Sdk.Account;

public static class MyProfileMapper
{
    public static MyProfile FromDto(MyProfileDto dto)
    {
        return new MyProfile(dto.Id ?? Guid.Empty,
            dto.Username ?? string.Empty,
            dto.ContactEmail ?? string.Empty,
            dto.Properties?.Select(UserPropertyMapper.FromDto).ToImmutableArray() ??
            ImmutableArray<UserProperty>.Empty);
    }
    public static MyProfileDto ToDto(MyProfile myProfile)
    {
        return new MyProfileDto
        {
            Id = myProfile.Id,
            Properties = myProfile.Properties.Select(UserPropertyMapper.ToDto).ToArray(),
            ContactEmail = myProfile.ContactEmail,
            Username = myProfile.Username
        };
    }
}