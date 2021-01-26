using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Qweree.Utils
{
    public static class HttpContentExtensions
    {
        public static async Task<TValueType?> ReadAsObjectAsync<TValueType>(this HttpContent httpContent,
            JsonSerializerOptions jsonOptions, CancellationToken cancellationToken = new()) where TValueType : class
        {
            var stream = await httpContent.ReadAsStreamAsync(cancellationToken);
            return await JsonUtils.DeserializeAsync<TValueType>(stream, jsonOptions, cancellationToken);
        }

        public static async Task<TValueType?> ReadAsObjectAsync<TValueType>(this HttpContent httpContent,
            CancellationToken cancellationToken = new()) where TValueType : class
        {
            var stream = await httpContent.ReadAsStreamAsync(cancellationToken);
            return await JsonUtils.DeserializeAsync<TValueType>(stream, cancellationToken);
        }
    }
}