using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Qweree.Authentication.Sdk.Account;
using Qweree.Authentication.WebApi.Domain.Authentication;
using Qweree.Authentication.WebApi.Domain.Identity;
using Qweree.Authentication.WebApi.Infrastructure.Identity;
using Qweree.Authentication.WebApi.Infrastructure.Security;
using Qweree.Authentication.WebApi.Test.Fixture;
using Qweree.Authentication.WebApi.Test.Fixture.Factories;
using Qweree.Utils;
using Xunit;

namespace Qweree.Authentication.WebApi.Test.Web.Account;

[Collection("Web api collection")]
[Trait("Category", "Integration test")]
[Trait("Category", "Web api test")]
public class PasswordControllerTest : IClassFixture<WebApiFactory>, IDisposable
{
    private readonly WebApiFactory _webApiFactory;
    private readonly HttpClient _client;
    private readonly IServiceScope _scope;
    private readonly UserRepository _userRepository;
    private readonly ClientRepository _clientRepository;

    public PasswordControllerTest(WebApiFactory webApiFactory)
    {
        _webApiFactory = webApiFactory;
        _client = webApiFactory.CreateClient();
        _scope = webApiFactory.Services.CreateScope();

        _userRepository = (UserRepository) _scope.ServiceProvider.GetRequiredService<IUserRepository>();
        _userRepository.DeleteAllAsync()
            .GetAwaiter()
            .GetResult();
        _clientRepository = (ClientRepository) _scope.ServiceProvider.GetRequiredService<IClientRepository>();
        _clientRepository.DeleteAllAsync()
            .GetAwaiter()
            .GetResult();
    }

    public void Dispose()
    {
        _client.Dispose();
        _scope.Dispose();
    }

    [Fact]
    public async Task TestChangeMyPassword()
    {
        var user = UserFactory.CreateDefault();
        await _userRepository.InsertAsync(user);
        var client = ClientFactory.CreateDefault(user.Id);
        await _clientRepository.InsertAsync(client);


        {
            using var httpClient = await _webApiFactory.CreateAuthenticatedClientAsync(new Sdk.OAuth2.ClientCredentials(client.ClientId, client.ClientSecret),
                new Sdk.OAuth2.PasswordGrantInput(user.Username, user.Password));
            var input = new ChangeMyPasswordInputDto
            {
                OldPassword = user.Password,
                NewPassword = "xxx---pwd1122POsd"
            };

            var response = await httpClient.PostAsync("/api/account/change-password",
                new StringContent(JsonUtils.Serialize(input), Encoding.UTF8, MediaTypeNames.Application.Json));

            response.EnsureSuccessStatusCode();
        }

        {
            var input = new[]
            {
                new KeyValuePair<string?, string?>("grant_type", "password"),
                new KeyValuePair<string?, string?>("username", user.Username),
                new KeyValuePair<string?, string?>("password", user.Password),
            };

            var authHeaderEncoder = new AuthorizationHeaderEncoder();
            var authHeader = authHeaderEncoder.Encode(new ClientCredentials(client.ClientId, client.ClientSecret));
            var request = new FormUrlEncodedContent(input);
            var message = new HttpRequestMessage(HttpMethod.Post, "/api/oauth2/auth")
            {
                Content = request,
                Headers =
                {
                    {HeaderNames.Authorization, new[] {$"Basic {authHeader}"}}
                }
            };

            var response = await _client.SendAsync(message);
            Assert.False(response.IsSuccessStatusCode);
        }

        {
            var input = new[]
            {
                new KeyValuePair<string?, string?>("grant_type", "password"),
                new KeyValuePair<string?, string?>("username", user.Username),
                new KeyValuePair<string?, string?>("password", "xxx---pwd1122POsd"),
            };

            var authHeaderEncoder = new AuthorizationHeaderEncoder();
            var authHeader = authHeaderEncoder.Encode(new ClientCredentials(client.ClientId, client.ClientSecret));
            var request = new FormUrlEncodedContent(input);
            var message = new HttpRequestMessage(HttpMethod.Post, "/api/oauth2/auth")
            {
                Content = request,
                Headers =
                {
                    {HeaderNames.Authorization, new[] {$"Basic {authHeader}"}}
                }
            };

            var response = await _client.SendAsync(message);
            Assert.True(response.IsSuccessStatusCode);
        }
    }
}