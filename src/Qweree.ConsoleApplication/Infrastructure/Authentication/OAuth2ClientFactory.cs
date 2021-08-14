using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Authentication.Sdk.OAuth2;
using Qweree.ConsoleApplication.Infrastructure.RunContext;
using Qweree.Sdk.Http.HttpClient;

namespace Qweree.ConsoleApplication.Infrastructure.Authentication
{
    public class OAuth2ClientFactory
    {
        private readonly Context _context;
        private readonly QwereeHttpHandler _qwereeHandler;

        public OAuth2ClientFactory(HttpMessageHandler innerHandler, ITokenStorage tokenStorage, Context context)
        {
            _context = context;
            _qwereeHandler = new QwereeHttpHandler(innerHandler, tokenStorage);
        }

        public async Task<OAuth2Client> CreateClientAsync(CancellationToken cancellationToken = new())
        {
            var config = await _context.GetConfigurationAsync(cancellationToken);
            var httpClient = new HttpClient(_qwereeHandler)
            {
                BaseAddress = new Uri(new Uri(config.AuthUri!), "api/oauth2/auth/")
            };

            return new OAuth2Client(httpClient);
        }
    }
}