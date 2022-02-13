
using Qweree.Authentication.Sdk.Session;

namespace Qweree.Cdn.Sdk.Extensions;

public static class SessionStorageExtensions
{
    public static string GetUserRootPath(this ISessionStorage @this)
    {
        var userId = @this.CurrentUser?.Username;
        var clientId = @this.CurrentClient.ClientId;

        if (userId == null && string.IsNullOrWhiteSpace(clientId))
            throw new ArgumentException("Client id is empty.");
        if (userId == null)
            return PathHelper.GetClientRootPath(clientId);

        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("User id is empty.");

        return PathHelper.GetUserRootPath(userId);
    }

    public static string GetUserDataPath(this ISessionStorage @this)
    {
        if (@this.CurrentUser == null)
            return PathHelper.GetClientDataPath(@this.CurrentClient.ClientId);

        return PathHelper.GetUserDataPath(@this.CurrentUser.Username, @this.CurrentClient.ClientId);
    }
}