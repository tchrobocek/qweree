using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
using User = Qweree.Authentication.Sdk.Identity.User;
using UserMapper = Qweree.Authentication.Sdk.Identity.UserMapper;

namespace Qweree.Authentication.WebApi.Test.Web.Identity
{
    [Collection("Web api collection")]
    [Trait("Category", "Integration test")]
    [Trait("Category", "Web api test")]
    public class UserControllerTest : IClassFixture<WebApiFactory>
    {
        private readonly WebApiFactory _webApiFactory;
        private readonly UserRepository _userRepository;

        public UserControllerTest(WebApiFactory webApiFactory)
        {
            _webApiFactory = webApiFactory;

            using var scope = webApiFactory.Services.CreateScope();
            _userRepository = (UserRepository)scope.ServiceProvider.GetRequiredService<IUserRepository>();
            _userRepository.DeleteAllAsync()
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

        [Fact]
        public async Task TestPagination()
        {
            var usersList = new List<User>();

            for (var i = 0; i < 10; i++)
            {
                var user = UserFactory.CreateDefault($"{i}user");
                usersList.Add(new User(user.Id, user.Username, user.Roles));
                await _userRepository.InsertAsync(user);
            }

            usersList = usersList.OrderBy(u => u.Username).ToList();

            using var client = await _webApiFactory.CreateAuthenticatedClientAsync(UserFactory.CreateAdmin(), UserFactory.Password);

            {
                var response = await client.GetAsync($"/api/v1/identity/users?sort[Username]=1&skip=2&take=3");
                response.EnsureSuccessStatusCode();
                var userDtos = await response.Content.ReadAsObjectAsync<UserDto[]>();
                var users = userDtos!.Select(UserMapper.FromDto);

                users.ShouldDeepEqual(usersList.Skip(2).Take(3));
            }
        }

        [Fact]
        public async Task TestDelete()
        {
            var adminUser = UserFactory.CreateAdmin();
            using var client = await _webApiFactory.CreateAuthenticatedClientAsync(adminUser, UserFactory.Password);

            var user = UserFactory.CreateDefault();
            await _userRepository.InsertAsync(user);

            {
                var response = await client.GetAsync($"/api/v1/identity/users/{user!.Id}");
                response.EnsureSuccessStatusCode();
            }

            {
                var response = await client.DeleteAsync($"/api/v1/identity/users/{user!.Id}");
                response.EnsureSuccessStatusCode();
            }

            {
                var response = await client.GetAsync($"/api/v1/identity/users/{user!.Id}");
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }
    }
}