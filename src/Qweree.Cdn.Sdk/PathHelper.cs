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

    public static string GetUserRootPath(string username)
    {
        if (string.IsNullOrEmpty(username))
            throw new ArgumentException("User id is empty.");

        return $"/usr/{username}/";
    }

    public static string GetClientRootPath(string clientId)
    {
        if (string.IsNullOrEmpty(clientId))
            throw new ArgumentException("Client id is empty.");

        return $"/apps/{clientId}/";
    }

    public static string GetUserDataPath(string username, string clientId)
    {
        if (string.IsNullOrEmpty(username))
            throw new ArgumentException("User id is empty.");

        if (string.IsNullOrEmpty(clientId))
            throw new ArgumentException("Client id is empty.");

        return Combine(GetUserRootPath(username), "apps", clientId);
    }

    public static string GetClientDataPath(string clientId)
    {
        if (string.IsNullOrEmpty(clientId))
            throw new ArgumentException("Client id is empty.");

        return Combine(GetClientRootPath(clientId), "data");
    }
}