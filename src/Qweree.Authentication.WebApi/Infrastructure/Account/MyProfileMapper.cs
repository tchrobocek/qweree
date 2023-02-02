using System.Linq;
using Qweree.Authentication.WebApi.Domain.Account;
using Qweree.Authentication.WebApi.Infrastructure.Session;
using SdkMyProfile = Qweree.Authentication.Sdk.Account.MyProfile;

namespace Qweree.Authentication.WebApi.Infrastructure.Account;

public static class MyProfileMapper
{
    public static SdkMyProfile Map(MyProfile myProfile)
    {
        return new SdkMyProfile
        {
            Id = myProfile.Id,
            Properties = myProfile.Properties.Select(IdentityMapper.Map).ToArray(),
            ContactEmail = myProfile.ContactEmail,
            Username = myProfile.Username
        };
    }
}