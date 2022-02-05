using Qweree.AspNet.Session;

namespace Qweree.Cdn.Sdk.Extensions;

public static class SessionStorageExtensions
{
    public static string GetUserRootPath(this ISessionStorage @this)
    {
        var userId = @this.CurrentUser?.Id;
        var clientId = @this.CurrentClient.ClientId;

        if (userId == null && clientId == Guid.Empty)
            throw new ArgumentException("Client id is empty.");
        if (userId == null)
            return PathHelper.GetClientRootPath(clientId);

        if (userId == Guid.Empty)
            throw new ArgumentException("User id is empty.");

        return PathHelper.GetUserRootPath(userId.Value);
    }

    public static string GetUserDataPath(this ISessionStorage @this)
    {
        if (@this.CurrentUser == null)
            return PathHelper.GetClientDataPath(@this.CurrentClient.ClientId);

        return PathHelper.GetUserDataPath(@this.CurrentUser.Id, @this.CurrentClient.ClientId);
    }
}