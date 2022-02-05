using System;
using System.Linq;

namespace Qweree.Cdn.Sdk;

public static class PathHelper
{
    public static string[] PathToSlug(string path)
    {
        return path.Split("/", StringSplitOptions.RemoveEmptyEntries);
    }

    public static string SlugToPath(string[] slug)
    {
        return $"/{string.Join("/", slug)}";
    }

    public static string Combine(params string[] paths)
    {
        if (paths.Length == 0)
            return "/";

        return string.Join("/", paths.Select(p => p.Trim('/')));
    }

    public static string GetUserRootPath(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User id is empty.");

        return $"/usr/{userId}/";
    }

    public static string GetClientRootPath(Guid clientId)
    {
        if (clientId == Guid.Empty)
            throw new ArgumentException("Client id is empty.");

        return $"/apps/{clientId}/";
    }

    public static string GetUserDataPath(Guid userId, Guid clientId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User id is empty.");

        if (clientId == Guid.Empty)
            throw new ArgumentException("Client id is empty.");

        return Combine(GetUserRootPath(userId), "apps", clientId.ToString());
    }

    public static string GetClientDataPath(Guid clientId)
    {
        if (clientId == Guid.Empty)
            throw new ArgumentException("Client id is empty.");

        return Combine(GetClientRootPath(clientId), "data");
    }
}