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
using Qweree.Authentication.AdminSdk.Identity.Users;
using Qweree.Authentication.WebApi.Domain;
using Qweree.Authentication.WebApi.Domain.Identity;
using Qweree.Authentication.WebApi.Infrastructure.Identity;
using Qweree.Authentication.WebApi.Test.Fixture;
using Qweree.Authentication.WebApi.Test.Fixture.Factories;
using Qweree.TestUtils.DeepEqual;
using Qweree.Utils;
using Xunit;
using User = Qweree.Authentication.AdminSdk.Identity.Users.User;
using UserMapper = Qweree.Authentication.AdminSdk.Identity.Users.UserMapper;

namespace Qweree.Authentication.WebApi.Test.Web.Identity
{
    [Collection("Web api collection")]
    [Trait("Category", "Integration test")]
    [Trait("Category", "Web api test")]
    public class UserControllerTest : IClassFixture<WebApiFactory>
    {
        private readonly UserRepository _userRepository;
        private readonly SdkMapperService _sdkMapperService;
        private readonly WebApiFactory _webApiFactory;

        public UserControllerTest(WebApiFactory webApiFactory)
        {
            _webApiFactory = webApiFactory;

            using var scope = webApiFactory.Services.CreateScope();
            _sdkMapperService = scope.ServiceProvider.GetRequiredService<SdkMapperService>();
            _userRepository = (UserRepository) scope.ServiceProvider.GetRequiredService<IUserRepository>();
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
                var response = await client.PostAsync("/api/admin/identity/users",
                    new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json));
                response.EnsureSuccessStatusCode();
                user = await response.Content.ReadAsObjectAsync<UserDto>() ?? throw new ArgumentNullException();

                Assert.NotNull(user);
                Assert.Equal(input.Username, user.Username);
                Assert.Equal(input.Roles, user.Roles);
            }

            {
                var response = await client.GetAsync($"/api/admin/identity/users/{user!.Id}");
                response.EnsureSuccessStatusCode();
                var actualUser = await response.Content.ReadAsObjectAsync<UserDto>();

                actualUser.WithDeepEqual(user)
                    .WithCustomComparison(new MillisecondDateTimeComparison())
                    .Assert();
            }
        }

        [Fact]
        public async Task TestPagination()
        {
            var usersList = new List<User>();

            for (var i = 0; i < 10; i++)
            {
                var user = UserFactory.CreateDefault($"{i}user");
                usersList.Add(_sdkMapperService.MapUser(user));
                await _userRepository.InsertAsync(user);
            }

            usersList = usersList.OrderBy(u => u.Username).ToList();

            using var client =
                await _webApiFactory.CreateAuthenticatedClientAsync(UserFactory.CreateAdmin(), UserFactory.Password);

            {
                var response = await client.GetAsync("/api/admin/identity/users?sort[Username]=1&skip=2&take=3");
                response.EnsureSuccessStatusCode();
                var userDtos = await response.Content.ReadAsObjectAsync<UserDto[]>();
                var users = userDtos!.Select(UserMapper.FromDto);

                users.WithDeepEqual(usersList.Skip(2).Take(3))
                    .WithCustomComparison(new MillisecondDateTimeComparison())
                    .Assert();
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
                var response = await client.GetAsync($"/api/admin/identity/users/{user!.Id}");
                response.EnsureSuccessStatusCode();
            }

            {
                var response = await client.DeleteAsync($"/api/admin/identity/users/{user!.Id}");
                response.EnsureSuccessStatusCode();
            }

            {
                var response = await client.GetAsync($"/api/admin/identity/users/{user!.Id}");
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }
    }
}