using System;
using System.Collections.Immutable;
using System.Linq;
using Qweree.Authentication.WebApi.Domain.Identity;
using Client = Qweree.Authentication.WebApi.Domain.Identity.Client;
using SdkClientCreateInput = Qweree.Authentication.AdminSdk.Identity.Clients.ClientCreateInput;
using SdkClientModifyInput = Qweree.Authentication.AdminSdk.Identity.Clients.ClientModifyInput;
using SdkIAccessDefinitionInput = Qweree.Authentication.AdminSdk.Identity.Clients.IAccessDefinitionInput;
using SdkPasswordAccessDefinitionInput = Qweree.Authentication.AdminSdk.Identity.Clients.PasswordAccessDefinitionInput;
using SdkClientCredentialsAccessDefinitionInput = Qweree.Authentication.AdminSdk.Identity.Clients.ClientCredentialsAccessDefinitionInput;

namespace Qweree.Authentication.WebApi.Infrastructure.Identity;

public static class ClientMapper
{
    public static ClientCreateInput ToClientCreateInput(SdkClientCreateInput input)
    {
        return new ClientCreateInput(input.Id ?? Guid.Empty,
            input.ClientId ?? string.Empty,
            input.ApplicationName ?? string.Empty,
            input.Origin ?? string.Empty,
            input.OwnerId ?? Guid.Empty,
            input.AccessDefinitions?.Select(ToAccessDefinitionInput).ToImmutableArray() ?? ImmutableArray<IAccessDefinitionInput>.Empty);
    }

    public static IAccessDefinitionInput ToAccessDefinitionInput(SdkIAccessDefinitionInput input)
    {
        if (input is SdkPasswordAccessDefinitionInput)
            return new PasswordDefinitionInput();
        if (input is SdkClientCredentialsAccessDefinitionInput clientCredentials)
            return new ClientCredentialsDefinitionInput(clientCredentials.Roles?.ToImmutableArray() ?? ImmutableArray<Guid>.Empty);

        throw new ArgumentOutOfRangeException(nameof(input));
    }

    public static ClientModifyInput ToClientModifyInput(SdkClientModifyInput input)
    {
        return new ClientModifyInput(input.ApplicationName, input.Origin);
    }

    public static Client ToClient(ClientDo document)
    {
        return new Client(document.Id ?? Guid.Empty, document.ClientId ?? string.Empty, document.ClientSecret ?? string.Empty,
            document.ApplicationName ?? string.Empty, document.AccessDefinitions?.Select(ToAccessDefinition).ToImmutableArray() ?? ImmutableArray<IAccessDefinition>.Empty,
            document.CreatedAt ?? DateTime.MinValue, document.ModifiedAt ?? DateTime.MinValue,
            document.OwnerId ?? Guid.Empty, document.Origin ?? String.Empty);
    }

    private static IAccessDefinition ToAccessDefinition(AccessDefinitionDo definition)
    {
        if (definition is PasswordAccessDefinitionDo)
            return new PasswordAccessDefinition();

        if (definition is ClientCredentialsAccessDefinitionDo clientCredentials)
            return new ClientCredentialsAccessDefinition(clientCredentials.Roles?.ToImmutableArray() ?? ImmutableArray<Guid>.Empty);

        throw new ArgumentOutOfRangeException(nameof(definition));
    }

    public static ClientDo ToClientDo(Client client)
    {
        return new ClientDo
        {
            Id = client.Id,
            ClientId = client.ClientId,
            ClientSecret = client.ClientSecret,
            ApplicationName = client.ApplicationName,
            CreatedAt = client.CreatedAt,
            ModifiedAt = client.ModifiedAt,
            OwnerId = client.OwnerId,
            Origin = client.Origin,
            AccessDefinitions = client.AccessDefinitions.Select(ToAccessDefinitionDo).ToArray()
        };
    }

    private static AccessDefinitionDo ToAccessDefinitionDo(IAccessDefinition accessDefinition)
    {
        if (accessDefinition is PasswordAccessDefinition)
            return new PasswordAccessDefinitionDo();
        if (accessDefinition is ClientCredentialsAccessDefinition clientCredentials)
            return new ClientCredentialsAccessDefinitionDo
            {
                Roles = clientCredentials.Roles.ToArray()
            };

        throw new ArgumentOutOfRangeException(nameof(accessDefinition));
    }
}