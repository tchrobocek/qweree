using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Authentication.Sdk.OAuth2;
using Qweree.ConsoleApplication.Infrastructure.RunContext;

namespace Qweree.ConsoleApplication.Infrastructure.Authentication
{
    public class OAuth2ClientFactory
    {
        private readonly Context _context;
        private readonly HttpMessageHandler _innerHandler;

        public OAuth2ClientFactory(HttpMessageHandler innerHandler, Context context)
        {
            _innerHandler = innerHandler;
            _context = context;
        }

        public async Task<OAuth2Client> CreateClientAsync(CancellationToken cancellationToken = new())
        {
            var config = await _context.GetConfigurationAsync(cancellationToken);
            var httpClient = new HttpClient(_innerHandler)
            {
                BaseAddress = new Uri(new Uri(config.AuthUri!), "api/oauth2/auth/")
            };

            return new OAuth2Client(httpClient);
        }
    }
}