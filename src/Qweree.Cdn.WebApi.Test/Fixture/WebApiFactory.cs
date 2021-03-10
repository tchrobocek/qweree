using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Qweree.Authentication.Sdk.OAuth2;
using Qweree.Cdn.WebApi.Infrastructure.Authentication;
using Qweree.Cdn.WebApi.Infrastructure.Storage;
using Qweree.Mongo;
using Qweree.TestUtils.IO;

namespace Qweree.Cdn.WebApi.Test.Fixture
{
    public class WebApiFactory : WebApplicationFactory<Startup>
    {
        protected override IHostBuilder CreateHostBuilder()
        {
            return base.CreateHostBuilder()
                .ConfigureServices(ConfigureServices);
        }

        private void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            var objectStorage = services.First(s => s.ServiceType == typeof(IObjectStorage));
            services.Remove(objectStorage);

            services.AddSingleton<TemporaryFolder>();
            services.AddSingleton<IObjectStorage>(p =>
            {
                var tempFolder = p.GetRequiredService<TemporaryFolder>();
                return new FileObjectStorage(tempFolder.Path);
            });

            services.AddSingleton(_ =>
                new MongoContext(Settings.Database.ConnectionString, Settings.Database.DatabaseName));
        }

        public async Task<HttpClient> CreateAuthenticatedClientAsync(string username, string password)
        {
            var authConfig = Services.GetRequiredService<IOptions<AuthenticationConfigurationDo>>().Value;
            var client = CreateClient();
            var adapter = new OAuth2Adapter(new Uri(authConfig.TokenUri!), new HttpClient());
            var tokenInfo = await adapter.SignInAsync(new PasswordGrantInput(username, password));
            client.DefaultRequestHeaders.Add(HeaderNames.Authorization, $"Bearer {tokenInfo.AccessToken}");

            return client;
        }
    }
}