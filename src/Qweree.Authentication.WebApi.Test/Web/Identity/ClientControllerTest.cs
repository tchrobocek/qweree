using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using DeepEqual.Syntax;
using Microsoft.Extensions.DependencyInjection;
using Qweree.Authentication.AdminSdk.Identity.Clients;
using Qweree.Authentication.WebApi.Domain;
using Qweree.Authentication.WebApi.Domain.Identity;
using Qweree.Authentication.WebApi.Infrastructure.Identity;
using Qweree.Authentication.WebApi.Test.Fixture;
using Qweree.Authentication.WebApi.Test.Fixture.Factories;
using Qweree.Mongo.Exception;
using Qweree.TestUtils.DeepEqual;
using Qweree.Utils;
using Xunit;
using Client = Qweree.Authentication.AdminSdk.Identity.Clients.Client;
using ClientMapper = Qweree.Authentication.AdminSdk.Identity.Clients.ClientMapper;

namespace Qweree.Authentication.WebApi.Test.Web.Identity
{
    [Collection("Web api collection")]
    [Trait("Category", "Integration test")]
    [Trait("Category", "Web api test")]
    public class ClientControllerTest
    {
        private readonly ClientRepository _clientRepository;
        private readonly UserRepository _userRepository;
        private readonly SdkMapperService _sdkMapperService;
        private readonly WebApiFactory _webApiFactory;

        public ClientControllerTest(WebApiFactory webApiFactory)
        {
            _webApiFactory = webApiFactory;

            using var scope = webApiFactory.Services.CreateScope();
            _sdkMapperService = scope.ServiceProvider.GetRequiredService<SdkMapperService>();
            _clientRepository = (ClientRepository) scope.ServiceProvider.GetRequiredService<IClientRepository>();
            _clientRepository.DeleteAllAsync()
                .GetAwaiter()
                .GetResult();
            _userRepository = (UserRepository) scope.ServiceProvider.GetRequiredService<IUserRepository>();
            _userRepository.DeleteAllAsync()
                .GetAwaiter()
                .GetResult();
        }

        [Fact]
        public async Task TestInsertAndGetClient()
        {
            var user = UserFactory.CreateAdmin();
            var client = ClientFactory.CreateDefault(user.Id);

            await _userRepository.InsertAsync(user);

            using var httpClient = await _webApiFactory.CreateAuthenticatedClientAsync(client, user);
            {
                var input = new ClientCreateInput(client.Id, client.ClientId, client.ApplicationName,
                    client.Origin, user.Id, ImmutableArray<Guid>.Empty);

                var json = JsonUtils.Serialize(input);
                var response = await httpClient.PostAsync("/api/admin/identity/clients",
                    new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json));

                response.EnsureSuccessStatusCode();

                var createdClientDto = await response.Content.ReadAsObjectAsync<CreatedClientDto>();

                createdClientDto.WithDeepEqual(CreatedClientMapper.ToDto(await _sdkMapperService.ClientMapToCreatedClientAsync(client)))
                    .WithCustomComparison(new MillisecondDateTimeComparison())
                    .WithCustomComparison(new ImmutableArrayComparison())
                    .IgnoreProperty(p => p.Name == nameof(client.ClientSecret))
                    .IgnoreProperty(p => p.Name == "CreatedAt" || p.Name == "ModifiedAt")
                    .Assert();
            }

            {
                var response = await httpClient.GetAsync($"/api/admin/identity/clients/{client.Id}");

                response.EnsureSuccessStatusCode();

                var clientDto = await response.Content.ReadAsObjectAsync<ClientDto>();

                clientDto.WithDeepEqual(ClientMapper.ToDto(await _sdkMapperService.ClientMapAsync(client)))
                    .WithCustomComparison(new MillisecondDateTimeComparison())
                    .WithCustomComparison(new ImmutableArrayComparison())
                    .IgnoreProperty(p => p.Name is "CreatedAt" or "ModifiedAt")
                    .Assert();
            }

            {
                var response = await httpClient.DeleteAsync($"/api/admin/identity/clients/{client.Id}");

                response.EnsureSuccessStatusCode();
            }

            {
                var response = await httpClient.GetAsync($"/api/admin/identity/clients/{client.Id}");

                Assert.False(response.IsSuccessStatusCode);
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }

            {
                await Assert.ThrowsAsync<DocumentNotFoundException>(async () => await _clientRepository.GetAsync(client.Id));
            }
        }

        [Fact]
        public async Task TestPagination()
        {
            var user = UserFactory.CreateAdmin();
            await _userRepository.InsertAsync(user);
            var clientsList = new List<Client>();

            for (var i = 0; i < 10; i++)
            {
                var client = ClientFactory.CreateDefault(user.Id, $"client{i}");
                clientsList.Add(await _sdkMapperService.ClientMapAsync(client));
                await _clientRepository.InsertAsync(client);
            }

            clientsList = clientsList.OrderBy(u => u.ClientId).ToList();

            var adminUser = UserFactory.CreateAdmin();
            var adminClient = ClientFactory.CreateDefault(adminUser.Id);

            using var httpClient =
                await _webApiFactory.CreateAuthenticatedClientAsync(adminClient, adminUser);

            {
                var response = await httpClient.GetAsync("/api/admin/identity/clients?sort[ClientId]=1&skip=2&take=3");
                response.EnsureSuccessStatusCode();
                var clientDtos = await response.Content.ReadAsObjectAsync<ClientDto[]>();

                clientDtos.WithDeepEqual(clientsList.Skip(2).Take(3).Select(ClientMapper.ToDto))
                    .WithCustomComparison(new MillisecondDateTimeComparison())
                    .WithCustomComparison(new ImmutableArrayComparison())
                    .Assert();
            }
        }

    }
}