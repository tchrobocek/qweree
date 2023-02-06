using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Qweree.AspNet.Application;
using Qweree.AspNet.Validations;
using Qweree.Authentication.WebApi.Domain.Security;
using Qweree.Authentication.WebApi.Domain.Session;
using Qweree.Mongo.Exception;
using Qweree.Utils;
using Qweree.Validator;

namespace Qweree.Authentication.WebApi.Domain.Identity;

public class ClientService
{
    private readonly IValidator _validator;
    private readonly IPasswordEncoder _passwordEncoder;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IClientRepository _clientRepository;
    private readonly ISessionInfoRepository _sessionInfoRepository;

    public ClientService(IValidator validator, IPasswordEncoder passwordEncoder, IDateTimeProvider dateTimeProvider,
        IClientRepository clientRepository,
        ISessionInfoRepository sessionInfoRepository)
    {
        _validator = validator;
        _passwordEncoder = passwordEncoder;
        _dateTimeProvider = dateTimeProvider;
        _clientRepository = clientRepository;
        _sessionInfoRepository = sessionInfoRepository;
    }

    private string GenerateClientSecret()
    {
        const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789$~&*#@";
        const int secretLength = 40;
        var result = string.Empty;

        for (var i = 0; i < secretLength; i++)
        {
            var index = RandomNumberGenerator.GetInt32(chars.Length);
            result += chars[index];
        }

        return result;
    }

    public async Task<Response<ClientSecretPair>> ClientCreateAsync(ClientCreateInput clientCreateInput,
        CancellationToken cancellationToken = new())
    {
        var validationResult = await _validator.ValidateAsync(clientCreateInput, cancellationToken);
        if (validationResult.HasFailed)
            return validationResult.ToErrorResponse<ClientSecretPair>();

        var id = clientCreateInput.Id;
        if (id == Guid.Empty)
            id = Guid.NewGuid();

        var clientSecret = GenerateClientSecret();
        var secret = _passwordEncoder.EncodePassword(clientSecret);

        var client = new Client(id, clientCreateInput.ClientId, secret,
            clientCreateInput.ApplicationName, ImmutableArray<IAccessDefinition>.Empty,
            _dateTimeProvider.UtcNow, _dateTimeProvider.UtcNow,
            clientCreateInput.OwnerId, clientCreateInput.Origin);

        try
        {
            await _clientRepository.InsertAsync(client, cancellationToken);
        }
        catch (InsertDocumentException)
        {
            return Response.Fail<ClientSecretPair>("Client is a duplicate.");
        }

        return Response.Ok(new ClientSecretPair(client, secret));
    }

    public async Task<Response<Client>> ClientGetAsync(Guid id,
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

    public async Task<PaginationResponse<Client>> ClientPaginateAsync(int skip, int take, Dictionary<string, int> sort,
        CancellationToken cancellationToken = new())
    {
        var pagination = await _clientRepository.PaginateAsync(skip, take, sort, cancellationToken);
        return Response.Ok(pagination.Documents, pagination.TotalCount);
    }

    public async Task<CollectionResponse<SessionInfo>> ClientGetActiveSessions(Guid id, CancellationToken cancellationToken = new())
    {
        return Response.Ok(await _sessionInfoRepository.FindActiveSessionsForUser(id, cancellationToken));
    }

    public async Task<Response> ClientDeleteAsync(Guid id)
    {
        await _clientRepository.DeleteOneAsync(id);
        return Response.Ok();
    }

    public async Task<Response<Client>> ClientModifyAsync(Guid id, ClientModifyInput input, CancellationToken cancellationToken = new())
    {
        var validationResult = await _validator.ValidateAsync(input, cancellationToken);
        if (validationResult.HasFailed)
            return validationResult.ToErrorResponse<Client>();

        Client client;

        try
        {
            client = await _clientRepository.GetAsync(id, cancellationToken);
        }
        catch (DocumentNotFoundException)
        {
            return Response.Fail<Client>(new Error("Client was not found", 404));
        }

        client = new Client(client.Id, client.ClientId, client.ClientSecret, input.ApplicationName ?? client.ApplicationName,
            client.AccessDefinitions, client.CreatedAt, _dateTimeProvider.UtcNow, client.OwnerId,  input.Origin ?? client.Origin);

        await _clientRepository.ReplaceAsync(client.Id.ToString(), client, cancellationToken);

        return Response.Ok(client);
    }

    public async Task<Response<ClientSecretPair>> ClientSecretRegenerateAsync(Guid id, CancellationToken cancellationToken = new())
    {
        Client client;

        try
        {
            client = await _clientRepository.GetAsync(id, cancellationToken);
        }
        catch (DocumentNotFoundException)
        {
            return Response.Fail<ClientSecretPair>(new Error("Client was not found", 404));
        }

        var clientSecret = GenerateClientSecret();
        var secret = _passwordEncoder.EncodePassword(clientSecret);

        client = new Client(client.Id, client.ClientId, secret, client.ApplicationName,
            client.AccessDefinitions, client.CreatedAt, _dateTimeProvider.UtcNow, client.OwnerId, client.Origin);

        await _clientRepository.ReplaceAsync(client.Id.ToString(), client, cancellationToken);

        return Response.Ok(new ClientSecretPair(client, clientSecret));
    }

    public async Task<Response<Client>> AccessDefinitionsReplaceAsync(Guid id, IEnumerable<IAccessDefinitionInput> inputs, CancellationToken cancellationToken = new())
    {
        inputs = inputs.ToArray();
        var validationResult = await _validator.ValidateAsync(inputs, cancellationToken);
        if (validationResult.HasFailed)
            return validationResult.ToErrorResponse<Client>();

        Client client;

        try
        {
            client = await _clientRepository.GetAsync(id, cancellationToken);
        }
        catch (DocumentNotFoundException)
        {
            return Response.Fail<Client>(new Error("Client was not found", 404));
        }

        var definitions = new List<IAccessDefinition>();
        foreach (var input in inputs)
        {
            if (input is PasswordDefinitionInput)
            {
                definitions.Add(new PasswordAccessDefinition());
                continue;
            }

            if (input is ClientCredentialsDefinitionInput clientCredentials)
            {
                definitions.Add(new ClientCredentialsAccessDefinition(clientCredentials.Roles));
                continue;
            }

            if (input is ImplicitAccessDefinitionInput @implicit)
            {
                definitions.Add(new ImplicitAccessDefinition(@implicit.RedirectUri));
                continue;
            }

            if (input is AuthorizationCodeAccessDefinitionInput authorizationCode)
            {
                definitions.Add(new AuthorizationCodeAccessDefinition(authorizationCode.RedirectUri));
                continue;
            }

            throw new ArgumentOutOfRangeException(nameof(input));
        }

        client = new Client(client.Id, client.ClientId, client.ClientSecret, client.ApplicationName,
            definitions.ToImmutableArray(), client.CreatedAt, _dateTimeProvider.UtcNow, client.OwnerId, client.Origin);

        await _clientRepository.ReplaceAsync(client.Id.ToString(), client, cancellationToken);

        return Response.Ok(client);
    }
}