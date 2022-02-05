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
        return $"/usr/{userId}/";
    }

    public static string GetClientRootPath(Guid clientId)
    {
        return $"/apps/{clientId}/";
    }

    public static string GetUserDataPath(Guid userId, Guid clientId)
    {
        return Combine(GetUserRootPath(userId), "apps", clientId.ToString());
    }

    public static string GetClientDataPath(Guid clientId)
    {
        return Combine(GetClientRootPath(clientId), "data");
    }
}