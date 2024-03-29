using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Qweree.Authentication.Sdk.OAuth2;
using Qweree.Authentication.Sdk.Session.Tokens.Jwt;
using Qweree.Authentication.WebApi.Domain.Identity;
using Qweree.Authentication.WebApi.Domain.Session;
using Qweree.Authentication.WebApi.Infrastructure.Identity;
using Qweree.Authentication.WebApi.Infrastructure.Security;
using Qweree.Authentication.WebApi.Infrastructure.Session;
using Qweree.Authentication.WebApi.Test.Fixture;
using Qweree.Authentication.WebApi.Test.Fixture.Factories;
using Qweree.Mongo.Exception;
using Qweree.Utils;
using Xunit;
using ClientCredentials = Qweree.Authentication.WebApi.Domain.Authentication.ClientCredentials;

namespace Qweree.Authentication.WebApi.Test.Web.Authentication;

[Collection("Web api collection")]
[Trait("Category", "Integration test")]
[Trait("Category", "Web api test")]
public class OAuth2ControllerTest : IClassFixture<WebApiFactory>, IDisposable
{
    private readonly HttpClient _client;
    private readonly IServiceScope _scope;
    private readonly UserRepository _userRepository;
    private readonly ClientRepository _clientRepository;
    private readonly SessionInfoRepository _sessionInfoRepository;

    public OAuth2ControllerTest(WebApiFactory webApiFactory)
    {
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
        _sessionInfoRepository = (SessionInfoRepository) _scope.ServiceProvider.GetRequiredService<ISessionInfoRepository>();
        _sessionInfoRepository.DeleteAllAsync()
            .GetAwaiter()
            .GetResult();
    }

    public void Dispose()
    {
        _client.Dispose();
        _scope.Dispose();
    }

    [Fact]
    public async Task TestAuthenticatePassword_RequestBody()
    {
        var user = UserFactory.CreateDefault();
        await _userRepository.InsertAsync(user);
        var client = ClientFactory.CreateDefault(user.Id);
        await _clientRepository.InsertAsync(client);

        var input = new[]
        {
            new KeyValuePair<string?, string?>("grant_type", "password"),
            new KeyValuePair<string?, string?>("username", user.Username),
            new KeyValuePair<string?, string?>("password", user.Password),
            new KeyValuePair<string?, string?>("client_id", client.ClientId),
            new KeyValuePair<string?, string?>("client_secret", client.ClientSecret)
        };
        var request = new FormUrlEncodedContent(input);

        var response = await _client.PostAsync("/api/oauth2/auth", request);
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task TestAuthenticatePassword_Header()
    {
        var user = UserFactory.CreateDefault();
        await _userRepository.InsertAsync(user);
        var client = ClientFactory.CreateDefault(user.Id);
        await _clientRepository.InsertAsync(client);

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
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task TestRefresh()
    {
        var user = UserFactory.CreateDefault();
        await _userRepository.InsertAsync(user);
        var client = ClientFactory.CreateDefault(user.Id);
        await _clientRepository.InsertAsync(client);

        string refreshToken;

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
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var tokenInfo = JsonUtils.Deserialize<TokenInfo>(content, JsonUtils.SnakeCaseNamingPolicy);

            refreshToken = tokenInfo?.RefreshToken!;
        }

        {
            var input = new[]
            {
                new KeyValuePair<string?, string?>("grant_type", "refresh_token"),
                new KeyValuePair<string?, string?>("refresh_token", refreshToken),
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
            response.EnsureSuccessStatusCode();
        }
    }

    [Fact]
    public async Task TestRevoke()
    {
        var user = UserFactory.CreateDefault();
        await _userRepository.InsertAsync(user);
        var client = ClientFactory.CreateDefault(user.Id);
        await _clientRepository.InsertAsync(client);

        TokenInfo? tokenInfo;
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
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            tokenInfo = JsonUtils.Deserialize<TokenInfo>(content, JsonUtils.SnakeCaseNamingPolicy);
        }

        var encoder = new JwtEncoder(Startup.Issuer);
        var accessToken = encoder.DecodeAccessToken(tokenInfo?.AccessToken!);
        await _sessionInfoRepository.GetAsync((Guid)accessToken.SessionId!);

        {
            var message = new HttpRequestMessage(HttpMethod.Post, "/api/oauth2/revoke")
            {
                Content = new ByteArrayContent(Array.Empty<byte>()),
                Headers =
                {
                    {HeaderNames.Authorization, new[] {$"Bearer {tokenInfo?.AccessToken}"}}
                }
            };

            var response = await _client.SendAsync(message);
            response.EnsureSuccessStatusCode();
        }

        await Assert.ThrowsAsync<DocumentNotFoundException>(async () =>
        {
            await _sessionInfoRepository.GetAsync((Guid)accessToken.SessionId!);
        });
    }

    [Fact]
    public async Task TestAuthenticateClientCredentials()
    {
        var user = UserFactory.CreateDefault();
        await _userRepository.InsertAsync(user);
        var client = ClientFactory.CreateDefault(user.Id);
        await _clientRepository.InsertAsync(client);

        var input = new[]
        {
            new KeyValuePair<string?, string?>("grant_type", "client_credentials"),
            new KeyValuePair<string?, string?>("client_id", client.ClientId),
            new KeyValuePair<string?, string?>("client_secret", client.ClientSecret)
        };
        var request = new FormUrlEncodedContent(input);

        var response = await _client.PostAsync("/api/oauth2/auth", request);
        response.EnsureSuccessStatusCode();
    }
}