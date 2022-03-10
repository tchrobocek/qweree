using Qweree.Authentication.AdminSdk.Identity.Users.UserInvitation;
using Qweree.Authentication.WebApi.Domain.Persistence;

namespace Qweree.Authentication.WebApi.Domain.Identity.UserInvitation;

public interface IUserInvitationRepository : IRepository<UserInvitationDescriptor>
{

}