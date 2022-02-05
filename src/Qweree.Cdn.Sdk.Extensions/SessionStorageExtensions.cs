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
            return $"/apps/{clientId}/";

        if (userId == Guid.Empty)
            throw new ArgumentException("User id is empty.");

        return $"/usr/{userId}/";
    }

    public static string GetUserDataPath(this ISessionStorage @this)
    {
        var root = @this.GetUserRootPath();
        var dataFolder = "data";

        if (@this.CurrentUser != null)
            dataFolder = $"apps/{@this.CurrentClient.ClientId}";

        return PathHelper.Combine(root, dataFolder);
    }
}