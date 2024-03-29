using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DeepEqual.Syntax;
using Microsoft.Extensions.DependencyInjection;
using Qweree.Authentication.Sdk.OAuth2;
using Qweree.Authentication.WebApi.Domain.Identity;
using Qweree.Authentication.WebApi.Infrastructure;
using Qweree.Authentication.WebApi.Infrastructure.Identity;
using Qweree.Authentication.WebApi.Test.Fixture;
using Qweree.Authentication.WebApi.Test.Fixture.Factories;
using Qweree.TestUtils.DeepEqual;
using Qweree.Utils;
using Xunit;

namespace Qweree.Authentication.WebApi.Test.Web.Identity;

[Collection("Web api collection")]
[Trait("Category", "Integration test")]
[Trait("Category", "Web api test")]
[Trait("Category", "Database test")]
public class UserControllerTest : IClassFixture<WebApiFactory>
{
    private readonly UserRepository _userRepository;
    private readonly ClientRepository _clientRepository;
    private readonly AdminSdkMapperService _sdkMapperService;
    private readonly WebApiFactory _webApiFactory;

    public UserControllerTest(WebApiFactory webApiFactory)
    {
        _webApiFactory = webApiFactory;

        using var scope = webApiFactory.Services.CreateScope();
        _sdkMapperService = scope.ServiceProvider.GetRequiredService<AdminSdkMapperService>();
        _userRepository = (UserRepository) scope.ServiceProvider.GetRequiredService<IUserRepository>();
        _userRepository.DeleteAllAsync()
            .GetAwaiter()
            .GetResult();
        _clientRepository = (ClientRepository) scope.ServiceProvider.GetRequiredService<IClientRepository>();
        _clientRepository.DeleteAllAsync()
            .GetAwaiter()
            .GetResult();
    }

    [Fact]
    public async Task TestGetUser()
    {
        var adminUser = UserFactory.CreateAdmin();
        var client = ClientFactory.CreateDefault(adminUser.Id);

        await _userRepository.InsertAsync(adminUser);
        await _clientRepository.InsertAsync(client);

        using var httpClient = await _webApiFactory.CreateAuthenticatedClientAsync(new ClientCredentials
            {
                ClientId = client.ClientId,
                ClientSecret = client.ClientSecret
            },
            new PasswordGrantInput
            {
                Username = adminUser.Username,
                Password = adminUser.Password
            });

        {
            var response = await httpClient.GetAsync($"/api/admin/identity/users/{adminUser.Id}");
            response.EnsureSuccessStatusCode();
            var actualUser = await response.Content.ReadAsObjectAsync<AdminSdk.Identity.Users.User>();

            actualUser.WithDeepEqual(await _sdkMapperService.ToUserAsync(adminUser))
                .WithCustomComparison(new MillisecondDateTimeComparison())
                .Assert();
        }
    }

    [Fact]
    public async Task TestPagination()
    {
        var usersList = new List<AdminSdk.Identity.Users.User>();

        for (var i = 0; i < 10; i++)
        {
            var user = UserFactory.CreateDefault($"{i}user");
            usersList.Add(await _sdkMapperService.ToUserAsync(user));
            await _userRepository.InsertAsync(user);
        }

        usersList = usersList.OrderBy(u => u.Username).ToList();
        var admin = UserFactory.CreateAdmin();
        var client = ClientFactory.CreateDefault(admin.Id);
        await _userRepository.InsertAsync(admin);
        await _clientRepository.InsertAsync(client);

        using var httpClient = await _webApiFactory.CreateAuthenticatedClientAsync(new ClientCredentials
            {
                ClientId = client.ClientId,
                ClientSecret = client.ClientSecret
            },
            new PasswordGrantInput
            {
                Username = admin.Username,
                Password = admin.Password
            });

        {
            var response = await httpClient.GetAsync("/api/admin/identity/users?sort[Username]=1&skip=2&take=3");
            response.EnsureSuccessStatusCode();
            var userDtos = await response.Content.ReadAsObjectAsync<AdminSdk.Identity.Users.User[]>();

            userDtos.WithDeepEqual(usersList.Skip(2).Take(3))
                .WithCustomComparison(new MillisecondDateTimeComparison())
                .IgnoreProperty(p => p.Name == nameof(User.Roles))
                .Assert();
        }
    }

    [Fact]
    public async Task TestDelete()
    {
        var adminUser = UserFactory.CreateAdmin();
        var client = ClientFactory.CreateDefault(adminUser.Id);
        await _userRepository.InsertAsync(adminUser);
        await _clientRepository.InsertAsync(client);
        using var httpClient = await _webApiFactory.CreateAuthenticatedClientAsync(new ClientCredentials
            {
                ClientId = client.ClientId,
                ClientSecret = client.ClientSecret
            },
            new PasswordGrantInput
            {
                Username = adminUser.Username,
                Password = adminUser.Password
            });

        var user = UserFactory.CreateDefault();
        await _userRepository.InsertAsync(user);

        {
            var response = await httpClient.GetAsync($"/api/admin/identity/users/{user.Id}");
            response.EnsureSuccessStatusCode();
        }

        {
            var response = await httpClient.DeleteAsync($"/api/admin/identity/users/{user.Id}");
            response.EnsureSuccessStatusCode();
        }

        {
            var response = await httpClient.GetAsync($"/api/admin/identity/users/{user.Id}");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}