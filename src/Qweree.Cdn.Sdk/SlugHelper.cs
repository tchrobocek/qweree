using System;

namespace Qweree.Cdn.Sdk
{
    public static class SlugHelper
    {
        public static string[] PathToSlug(string path)
        {
            return path.Split("/", StringSplitOptions.RemoveEmptyEntries);
        }

        public static string SlugToPath(string[] slug)
        {
            return $"/{string.Join("/", slug)}";
        }
    }
}