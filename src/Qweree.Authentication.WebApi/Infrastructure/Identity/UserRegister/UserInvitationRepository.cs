using System;
using Qweree.Authentication.AdminSdk.Identity.Users.UserRegister;
using Qweree.Authentication.WebApi.Domain.Identity.UserRegistration;
using Qweree.Mongo;

namespace Qweree.Authentication.WebApi.Infrastructure.Identity.UserRegister;

public class UserInvitationRepository : MongoRepositoryBase<UserInvitationDescriptor, UserInvitationDescriptorDo>, IUserInvitationRepository
{
    public UserInvitationRepository(MongoContext context) : base("user_invitations", context)
    {
    }

    protected override Func<UserInvitationDescriptor, UserInvitationDescriptorDo> ToDocument => UserInvitationMapper.ToDo;
    protected override Func<UserInvitationDescriptorDo, UserInvitationDescriptor> FromDocument => UserInvitationMapper.FromDo;
}