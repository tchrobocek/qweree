using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Qweree.AspNet.Application;
using Qweree.Authentication.AdminSdk.Identity.Clients;
using Qweree.Authentication.WebApi.Domain.Security;
using Qweree.Mongo.Exception;
using Qweree.Utils;
using Qweree.Validator;
using SdkClient = Qweree.Authentication.AdminSdk.Identity.Clients.Client;

namespace Qweree.Authentication.WebApi.Domain.Identity
{
    public class ClientService
    {
        private readonly IValidator _validator;
        private readonly IPasswordEncoder _passwordEncoder;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IClientRepository _clientRepository;
        private readonly SdkMapperService _sdkMapperService;
        private readonly Random _random;

        public ClientService(IValidator validator, IPasswordEncoder passwordEncoder, IDateTimeProvider dateTimeProvider,
            IClientRepository clientRepository, SdkMapperService sdkMapperService, Random random)
        {
            _validator = validator;
            _passwordEncoder = passwordEncoder;
            _dateTimeProvider = dateTimeProvider;
            _clientRepository = clientRepository;
            _sdkMapperService = sdkMapperService;
            _random = random;
        }

        private string GenerateClientSecret()
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789$~&*#@";
            const int secretLength = 40;
            var result = string.Empty;

            for (var i = 0; i < secretLength; i++)
            {
                var index = _random.Next(chars.Length);
                result += chars[index];
            }

            return result;
        }

        public async Task<Response<CreatedClient>> ClientCreateAsync(ClientCreateInput clientCreateInput,
            CancellationToken cancellationToken = new())
        {
            var validationResult = await _validator.ValidateAsync(clientCreateInput, cancellationToken);
            if (validationResult.HasFailed)
                return Response.Fail<CreatedClient>(validationResult.Errors.Select(e => $"{e.Path} - {e.Message}"));

            var id = clientCreateInput.Id;
            if (id == Guid.Empty)
                id = Guid.NewGuid();

            var clientSecret = GenerateClientSecret();
            var secret = _passwordEncoder.EncodePassword(clientSecret);
            var client = new Client(id, clientCreateInput.ClientId, secret,
                clientCreateInput.ApplicationName, clientCreateInput.ClientRoles, _dateTimeProvider.UtcNow, _dateTimeProvider.UtcNow,
                clientCreateInput.OwnerId, clientCreateInput.Origin);

            try
            {
                await _clientRepository.InsertAsync(client, cancellationToken);
            }
            catch (InsertDocumentException)
            {
                return Response.Fail<CreatedClient>("Client is a duplicate.");
            }

            var clientToReturn = new Client(client.Id, clientCreateInput.ClientId, clientSecret,
                clientCreateInput.ApplicationName, clientCreateInput.ClientRoles, _dateTimeProvider.UtcNow, _dateTimeProvider.UtcNow,
                clientCreateInput.OwnerId, clientCreateInput.Origin);
            return Response.Ok(await _sdkMapperService.ClientMapToCreatedClientAsync(clientToReturn, cancellationToken));
        }

        public async Task<Response<SdkClient>> ClientGetAsync(Guid id,
            CancellationToken cancellationToken = new())
        {
            Client client;

            try
            {
                client = await _clientRepository.GetAsync(id, cancellationToken);
            }
            catch (DocumentNotFoundException)
            {
                return Response.Fail<SdkClient>(new Error("Client was not found", 404));
            }

            return Response.Ok(await _sdkMapperService.ClientMapAsync(client, cancellationToken));
        }

        public async Task<PaginationResponse<SdkClient>> ClientPaginateAsync(int skip, int take, Dictionary<string, int> sort,
            CancellationToken cancellationToken = new())
        {
            var pagination = await _clientRepository.PaginateAsync(skip, take, sort, cancellationToken);
            var documents = new List<SdkClient>();

            foreach (var document in pagination.Documents)
            {
                documents.Add(await _sdkMapperService.ClientMapAsync(document, cancellationToken));
            }

            return Response.Ok(documents, pagination.TotalCount);
        }

        public async Task<Response> ClientDeleteAsync(Guid id)
        {
            await _clientRepository.DeleteAsync(id);
            return Response.Ok();
        }
    }
}