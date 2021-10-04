using System;
using Qweree.Authentication.AdminSdk.Identity.Users.UserRegister;
using Qweree.Mongo;

namespace Qweree.Authentication.WebApi.Infrastructure.Identity.UserRegister
{
    public class UserInvitationRepository : MongoRepositoryBase<UserInvitation, UserInvitationDo>
    {
        public UserInvitationRepository(MongoContext context) : base("user_invitations", context)
        {
        }

        protected override Func<UserInvitation, UserInvitationDo> ToDocument => UserInvitationMapper.ToDo;
        protected override Func<UserInvitationDo, UserInvitation> FromDocument => UserInvitationMapper.FromDo;
    }
}