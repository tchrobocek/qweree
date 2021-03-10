using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Qweree.AspNet.Application;
using Qweree.Authentication.WebApi.Domain.Security;
using Qweree.Mongo.Exception;
using Qweree.Utils;
using Qweree.Validator;

namespace Qweree.Authentication.WebApi.Domain.Identity
{
    public class ClientService
    {
        private readonly IValidator _validator;
        private readonly IPasswordEncoder _passwordEncoder;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IClientRepository _clientRepository;

        public ClientService(IValidator validator, IPasswordEncoder passwordEncoder, IDateTimeProvider dateTimeProvider,
            IClientRepository clientRepository)
        {
            _validator = validator;
            _passwordEncoder = passwordEncoder;
            _dateTimeProvider = dateTimeProvider;
            _clientRepository = clientRepository;
        }

        public async Task<Response<Client>> CreateClientAsync(ClientCreateInput clientCreateInput,
            CancellationToken cancellationToken = new())
        {
            var validationResult = await _validator.ValidateAsync(clientCreateInput, cancellationToken);
            if (validationResult.HasFailed)
                return Response.Fail<Client>(validationResult.Errors.Select(e => $"{e.Path} - {e.Message}"));

            var secret = _passwordEncoder.EncodePassword(clientCreateInput.ClientSecret);
            var client = new Client(clientCreateInput.Id, clientCreateInput.ClientId, secret,
                clientCreateInput.ApplicationName, _dateTimeProvider.UtcNow, _dateTimeProvider.UtcNow,
                clientCreateInput.OwnerId, clientCreateInput.Origin);

            try
            {
                await _clientRepository.InsertAsync(client, cancellationToken);
            }
            catch (InsertDocumentException)
            {
                return Response.Fail<Client>("Client is a duplicate.");
            }

            return Response.Ok(new Client(client.Id, clientCreateInput.ClientId, clientCreateInput.ClientSecret,
                    clientCreateInput.ApplicationName, _dateTimeProvider.UtcNow, _dateTimeProvider.UtcNow,
                    clientCreateInput.OwnerId, clientCreateInput.Origin));
        }

        public async Task<Response<Client>> GetClientAsync(Guid id,
            CancellationToken cancellationToken = new())
        {
            Client client;

            try
            {
                client = await _clientRepository.GetAsync(id, cancellationToken);
            }
            catch (DocumentNotFoundException)
            {
                return Response.Fail<Client>(new Error("Client was not found", 404));
            }

            return Response.Ok(client);
        }

        public async Task<PaginationResponse<Client>> PaginateClients(int skip, int take, Dictionary<string, int> sort,
            CancellationToken cancellationToken = new())
        {
            var pagination = await _clientRepository.PaginateAsync(skip, take, sort, cancellationToken);
            return Response.Ok(pagination.Documents, pagination.TotalCount);
        }
    }
}