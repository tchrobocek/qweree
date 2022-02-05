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
}