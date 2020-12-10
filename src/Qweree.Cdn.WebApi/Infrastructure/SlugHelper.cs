using System;

namespace Qweree.Cdn.WebApi.Infrastructure
{
    public class SlugHelper
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