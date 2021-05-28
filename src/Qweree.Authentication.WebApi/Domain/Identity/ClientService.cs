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

        public ClientService(IValidator validator, IPasswordEncoder passwordEncoder, IDateTimeProvider dateTimeProvider,
            IClientRepository clientRepository, SdkMapperService sdkMapperService)
        {
            _validator = validator;
            _passwordEncoder = passwordEncoder;
            _dateTimeProvider = dateTimeProvider;
            _clientRepository = clientRepository;
            _sdkMapperService = sdkMapperService;
        }

        public async Task<Response<CreatedClient>> CreateClientAsync(ClientCreateInput clientCreateInput,
            CancellationToken cancellationToken = new())
        {
            var validationResult = await _validator.ValidateAsync(clientCreateInput, cancellationToken);
            if (validationResult.HasFailed)
                return Response.Fail<CreatedClient>(validationResult.Errors.Select(e => $"{e.Path} - {e.Message}"));

            var secret = _passwordEncoder.EncodePassword(clientCreateInput.ClientSecret);
            var client = new Client(clientCreateInput.Id, clientCreateInput.ClientId, secret,
                clientCreateInput.ApplicationName, clientCreateInput.Roles, _dateTimeProvider.UtcNow, _dateTimeProvider.UtcNow,
                clientCreateInput.OwnerId, clientCreateInput.Origin);

            try
            {
                await _clientRepository.InsertAsync(client, cancellationToken);
            }
            catch (InsertDocumentException)
            {
                return Response.Fail<CreatedClient>("Client is a duplicate.");
            }

            var clientToReturn = new Client(client.Id, clientCreateInput.ClientId, clientCreateInput.ClientSecret,
                clientCreateInput.ApplicationName, clientCreateInput.Roles, _dateTimeProvider.UtcNow, _dateTimeProvider.UtcNow,
                clientCreateInput.OwnerId, clientCreateInput.Origin);
            return Response.Ok(await _sdkMapperService.MapToCreatedClientAsync(clientToReturn, cancellationToken));
        }

        public async Task<Response<SdkClient>> GetClientAsync(Guid id,
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

            return Response.Ok(await _sdkMapperService.MapClientAsync(client, cancellationToken));
        }

        public async Task<PaginationResponse<SdkClient>> PaginateClientsAsync(int skip, int take, Dictionary<string, int> sort,
            CancellationToken cancellationToken = new())
        {
            var pagination = await _clientRepository.PaginateAsync(skip, take, sort, cancellationToken);
            var documents = new List<SdkClient>();

            foreach (var document in pagination.Documents)
            {
                documents.Add(await _sdkMapperService.MapClientAsync(document, cancellationToken));
            }

            return Response.Ok(documents, pagination.TotalCount);
        }

        public async Task<Response> DeleteClientAsync(Guid id)
        {
            await _clientRepository.DeleteAsync(id);
            return Response.Ok();
        }
    }
}