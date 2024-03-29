using System;
using System.Collections.Immutable;

namespace Qweree.Authentication.WebApi.Domain.Identity;

public class ClientCreateInput
{
    public ClientCreateInput(Guid id, string clientId, string applicationName, string origin, Guid ownerId, ImmutableArray<IAccessDefinitionInput> accessDefinitions)
    {
        Id = id;
        ClientId = clientId;
        ApplicationName = applicationName;
        Origin = origin;
        OwnerId = ownerId;
        AccessDefinitions = accessDefinitions;
    }

    public Guid Id { get; }
    public string ClientId { get; }
    public string ApplicationName { get; }
    public Guid OwnerId { get; }
    public string Origin { get; }
    public ImmutableArray<IAccessDefinitionInput> AccessDefinitions { get; }
}