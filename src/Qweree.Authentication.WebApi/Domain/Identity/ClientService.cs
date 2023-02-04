using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Qweree.AspNet.Application;
using Qweree.AspNet.Validations;
using Qweree.Authentication.WebApi.Domain.Authorization;
using Qweree.Authentication.WebApi.Domain.Authorization.Roles;
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
    private readonly AuthorizationService _authorizationService;
    private readonly ISessionInfoRepository _sessionInfoRepository;
    private readonly Random _random;

    public ClientService(IValidator validator, IPasswordEncoder passwordEncoder, IDateTimeProvider dateTimeProvider,
        IClientRepository clientRepository, Random random, AuthorizationService authorizationService,
        ISessionInfoRepository sessionInfoRepository)
    {
        _validator = validator;
        _passwordEncoder = passwordEncoder;
        _dateTimeProvider = dateTimeProvider;
        _clientRepository = clientRepository;
        _random = random;
        _authorizationService = authorizationService;
        _sessionInfoRepository = sessionInfoRepository;
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
            clientCreateInput.ApplicationName, clientCreateInput.Roles,
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

    public async Task<Response<RolesCollection>> ClientGetEffectiveRolesAsync(Guid id,
        CancellationToken cancellationToken = new())
    {
        Client client;

        try
        {
            client = await _clientRepository.GetAsync(id, cancellationToken);
        }
        catch (DocumentNotFoundException)
        {
            return Response.Fail<RolesCollection>(new Error("Client was not found", 404));
        }

        var roles = new List<Role>();
        await foreach (var effectiveRole in _authorizationService.GetEffectiveRoles(client.Roles, cancellationToken)
                           .WithCancellation(cancellationToken))
        {
            roles.Add(effectiveRole);
        }

        return Response.Ok(new RolesCollection(roles.ToImmutableArray()));
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
}