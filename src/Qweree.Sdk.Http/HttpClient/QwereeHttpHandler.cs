using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Qweree.Sdk.Http.HttpClient
{
    public class QwereeHttpHandler : DelegatingHandler
    {
        private readonly ITokenStorage _tokenStorage;

        public QwereeHttpHandler(HttpMessageHandler innerHandler, ITokenStorage tokenStorage)
            : base(innerHandler)
        {
            _tokenStorage = tokenStorage;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string token;

            try
            {
                token = await _tokenStorage.GetAccessTokenAsync(cancellationToken);
            }
            catch (Exception)
            {
                token = string.Empty;
            }

            if (!string.IsNullOrEmpty(token))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return await base.SendAsync(request, cancellationToken);
        }
    }
}