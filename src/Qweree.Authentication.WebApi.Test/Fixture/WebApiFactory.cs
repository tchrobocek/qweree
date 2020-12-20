using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Moq;
using Qweree.AspNet.Application;
using Qweree.Authentication.WebApi.Application.Authentication;
using Qweree.Authentication.WebApi.Domain.Authentication;
using Qweree.Authentication.WebApi.Domain.Identity;
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

        public async Task<HttpClient> CreateAuthenticatedClientAsync(User user, string password)
        {
            var authConfig = Services.GetRequiredService<IOptions<AuthenticationConfigurationDo>>().Value;
            var client = CreateClient();
            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(m => m.GetByUsernameAsync(user.Username, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            var refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();

            var service = new AuthenticationService(userRepositoryMock.Object, refreshTokenRepositoryMock.Object, new DateTimeProvider(), new Random(), 7200, 7200, authConfig?.AccessTokenKey ?? "", authConfig?.FileAccessTokenKey ?? "", 0);
            var tokenInfoResponse = await service.AuthenticateAsync(new PasswordGrantInput(user.Username, password));

            Assert.Equal(ResponseStatus.Ok, tokenInfoResponse.Status);
            client.DefaultRequestHeaders.Add(HeaderNames.Authorization, $"Bearer {tokenInfoResponse.Payload?.AccessToken ?? ""}");

            return client;
        }
    }
}