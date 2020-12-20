using System;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using DeepEqual.Syntax;
using Microsoft.Extensions.DependencyInjection;
using Qweree.Authentication.Sdk.Identity;
using Qweree.Authentication.WebApi.Domain.Identity;
using Qweree.Authentication.WebApi.Infrastructure.Identity;
using Qweree.Authentication.WebApi.Test.Fixture;
using Qweree.Authentication.WebApi.Test.Fixture.Factories;
using Qweree.Utils;
using Xunit;

namespace Qweree.Authentication.WebApi.Test.Web.Identity
{
    public class UserControllerTest : IClassFixture<WebApiFactory>
    {
        private readonly WebApiFactory _webApiFactory;

        public UserControllerTest(WebApiFactory webApiFactory)
        {
            _webApiFactory = webApiFactory;

            using var scope = webApiFactory.Services.CreateScope();
            var userRepository = (UserRepository)scope.ServiceProvider.GetRequiredService<IUserRepository>();
            userRepository.DeleteAllAsync()
                .GetAwaiter()
                .GetResult();
        }

        [Fact]
        public async Task TestCreateAndGetUser()
        {
            var adminUser = UserFactory.CreateAdmin();
            using var client = await _webApiFactory.CreateAuthenticatedClientAsync(adminUser, UserFactory.Password);
            var input = new UserCreateInputDto
            {
                Username = "user",
                Password = "Password1",
                Roles = new[] {"role1"},
                ContactEmail = "user@example.com",
                FullName = "User Userov"
            };

            UserDto user;

            {
                var json = JsonUtils.Serialize(input);
                var response = await client.PostAsync("/api/v1/identity/users", new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json));
                response.EnsureSuccessStatusCode();
                user = await response.Content.ReadAsObjectAsync<UserDto>() ?? throw new ArgumentNullException();

                Assert.NotNull(user);
                Assert.Equal(input.Username, user.Username);
                Assert.Equal(input.Roles, user.Roles);
            }

            {
                var response = await client.GetAsync($"/api/v1/identity/users/{user!.Id}");
                response.EnsureSuccessStatusCode();
                var actualUser = await response.Content.ReadAsObjectAsync<UserDto>();

                actualUser.ShouldDeepEqual(user);
            }
        }
    }
}