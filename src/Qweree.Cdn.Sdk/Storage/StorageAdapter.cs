using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Sdk.Http.Errors;
using Qweree.Utils;

namespace Qweree.Cdn.Sdk.Storage
{
    public class StorageAdapter
    {
        private readonly Uri _cdnUri;
        private readonly IErrorHandler _errorHandler = new QwereeErrorHandler();
        private readonly HttpClient _httpClient;

        public StorageAdapter(Uri cdnUri, HttpClient httpClient)
        {
            _cdnUri = cdnUri;
            _httpClient = httpClient;
        }

        public async Task<StoredObjectDescriptor> StoreAsync(string path, string mediaType, Stream stream,
            CancellationToken cancellationToken = new())
        {
            var uri = new Uri(_cdnUri + $"/{path.Trim('/')}");
            var request = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = new StreamContent(stream)
                {
                    Headers =
                    {
                        {"Content-Type", mediaType}
                    }
                }
            };

            var response = await _httpClient.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
                await _errorHandler.HandleErrorResponseAsync(response, cancellationToken);

            var descriptor = await response.Content.ReadAsObjectAsync<StoredObjectDescriptorDto>(cancellationToken);
            return StoredObjectDescriptorMapper.FromDto(descriptor ?? throw new ArgumentException("Invalid response."));
        }

        public async Task<Stream> RetrieveAsync(string path, CancellationToken cancellationToken = new())
        {
            var uri = new Uri(_cdnUri + $"/{path.Trim('/')}");
            var response = await _httpClient.GetAsync(uri, cancellationToken);

            if (!response.IsSuccessStatusCode)
                await _errorHandler.HandleErrorResponseAsync(response, cancellationToken);

            return await response.Content.ReadAsStreamAsync(cancellationToken);
        }
    }
}