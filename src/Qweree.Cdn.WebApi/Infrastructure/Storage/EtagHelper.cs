using System.Security.Cryptography;
using System.Text;
using Qweree.Cdn.Sdk.Storage;

namespace Qweree.Cdn.WebApi.Infrastructure.Storage;

public static class EtagHelper
{
    public static string ComputeEtag(StoredObjectDescriptor descriptor)
    {
        var source = descriptor.ModifiedAt.ToString("O");

        using var sha = SHA256.Create();
        var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(source));

        var builder = new StringBuilder();

        foreach (var t in hash)
            builder.Append(t.ToString("x2"));

        // Return the hexadecimal string.
        return $@"""{builder}""";
    }

    public static bool ValidateEtag(string etag, StoredObjectDescriptor descriptor)
    {
        var actualEtag = ComputeEtag(descriptor);
        return etag == actualEtag;
    }
}