using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Qweree.Authentication.WebApi.Domain.Identity;
using Qweree.Authentication.WebApi.Infrastructure.Identity;
using Qweree.Authentication.WebApi.Test.Fixture;
using Qweree.Authentication.WebApi.Test.Fixture.Factories;
using Xunit;

namespace Qweree.Authentication.WebApi.Test.Web.Authentication
{
    [Collection("Web api collection")]
    [Trait("Category", "Integration test")]
    [Trait("Category", "Web api test")]
    public class OAuth2ControllerTest : IClassFixture<WebApiFactory>, IDisposable
    {
        private readonly HttpClient _client;
        private readonly IServiceScope _scope;
        private readonly UserRepository _userRepository;

        public OAuth2ControllerTest(WebApiFactory webApiFactory)
        {
            _client = webApiFactory.CreateClient();
            _scope = webApiFactory.Services.CreateScope();

            _userRepository = (UserRepository) _scope.ServiceProvider.GetRequiredService<IUserRepository>();
            _userRepository.DeleteAllAsync()
                .GetAwaiter()
                .GetResult();
        }

        public void Dispose()
        {
            _client.Dispose();
            _scope.Dispose();
        }

        [Fact]
        public async Task TestAuthenticatePassword()
        {
            var user = UserFactory.CreateDefault();
            await _userRepository.InsertAsync(user);

            var input = new[]
            {
                new KeyValuePair<string?, string?>("grant_type", "password"),
                new KeyValuePair<string?, string?>("username", user.Username),
                new KeyValuePair<string?, string?>("password", UserFactory.Password)
            };
            var request = new FormUrlEncodedContent(input);

            var response = await _client.PostAsync("/api/oauth2/auth", request);
            response.EnsureSuccessStatusCode();
        }
    }
}