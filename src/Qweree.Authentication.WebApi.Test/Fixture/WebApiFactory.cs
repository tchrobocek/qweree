using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Moq;
using Qweree.AspNet.Application;
using Qweree.Authentication.WebApi.Domain.Authentication;
using Qweree.Authentication.WebApi.Domain.Authorization;
using Qweree.Authentication.WebApi.Domain.Identity;
using Qweree.Authentication.WebApi.Domain.Security;
using Qweree.Authentication.WebApi.Infrastructure.Authentication;
using Qweree.Mongo;
using Qweree.Utils;
using Xunit;

namespace Qweree.Authentication.WebApi.Test.Fixture
{
    public class WebApiFactory : WebApplicationFactory<Startup>
    {
        private void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            var contextService = services.FirstOrDefault(s => s.ServiceType == typeof(MongoContext));
            services.Remove(contextService!);

            services.AddSingleton(_ =>
                new MongoContext(Settings.Database.ConnectionString, Settings.Database.DatabaseName));
        }

        protected override IHostBuilder CreateHostBuilder()
        {
            return base.CreateHostBuilder()
                .ConfigureServices(ConfigureServices);
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddSingleton<IPasswordEncoder, NonePasswordEncoder>();
            });
            base.ConfigureWebHost(builder);
        }

        public async Task<HttpClient> CreateAuthenticatedClientAsync(Client client, User user)
        {
            var authConfig = Services.GetRequiredService<IOptions<AuthenticationConfigurationDo>>().Value;
            var userRoleRepository = new UserRoleRepositoryMock();
            var httpClient = CreateClient();
            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(m => m.GetByUsernameAsync(user.Username, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var clientRepositoryMock = new Mock<IClientRepository>();
            clientRepositoryMock.Setup(m => m.GetByClientIdAsync(client.ClientId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(client);

            var refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();

            var service = new AuthenticationService(userRepositoryMock.Object, refreshTokenRepositoryMock.Object,
                new DateTimeProvider(), new Random(), 7200, 7200, authConfig?.AccessTokenKey ?? "",
                new NonePasswordEncoder(), clientRepositoryMock.Object, new AuthorizationService(userRoleRepository));
            var tokenInfoResponse = await service.AuthenticateAsync(new PasswordGrantInput(user.Username, user.Password), new ClientCredentials(client.ClientId, client.ClientSecret));

            Assert.Equal(ResponseStatus.Ok, tokenInfoResponse.Status);
            httpClient.DefaultRequestHeaders.Add(HeaderNames.Authorization,
                $"Bearer {tokenInfoResponse.Payload?.AccessToken ?? ""}");

            return httpClient;
        }
    }
}