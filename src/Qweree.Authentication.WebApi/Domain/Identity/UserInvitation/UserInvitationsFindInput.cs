using System.Collections.Generic;

namespace Qweree.Authentication.WebApi.Domain.Identity.UserInvitation;

public class UserInvitationsFindInput
{
    public UserInvitationsFindInput(int skip, int take, Dictionary<string, int> sort)
    {
        Skip = skip;
        Take = take;
        Sort = sort;
    }

    public int Skip { get; }
    public int Take { get; }
    public Dictionary<string, int> Sort { get; }
}