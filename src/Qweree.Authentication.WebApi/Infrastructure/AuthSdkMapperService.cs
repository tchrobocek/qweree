using System;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Authentication.WebApi.Domain.Identity;
using Qweree.Authentication.WebApi.Domain.Session;
using IClientInfo = Qweree.Authentication.WebApi.Domain.Session.IClientInfo;
using OperationSystemInfo = Qweree.Authentication.WebApi.Domain.Session.OperationSystemInfo;
using SdkClient = Qweree.Authentication.Sdk.Identity.Client;
using SdkSessionInfo = Qweree.Authentication.Sdk.Account.MyAccount.SessionInfo;
using SdkUserAgentInfo = Qweree.Authentication.Sdk.Account.MyAccount.UserAgentInfo;
using SdkIClientInfo = Qweree.Authentication.Sdk.Account.MyAccount.IClientInfo;
using SdkBotClientInfo = Qweree.Authentication.Sdk.Account.MyAccount.BotClientInfo;
using SdkBrowserClientInfo = Qweree.Authentication.Sdk.Account.MyAccount.BrowserClientInfo;
using SdkOperationSystemInfo = Qweree.Authentication.Sdk.Account.MyAccount.OperationSystemInfo;
using SessionInfo = Qweree.Authentication.WebApi.Domain.Session.SessionInfo;
using UserAgentInfo = Qweree.Authentication.WebApi.Domain.Session.UserAgentInfo;

namespace Qweree.Authentication.WebApi.Infrastructure;

public class AuthSdkMapperService
{
    private readonly IClientRepository _clientRepository;

    public AuthSdkMapperService(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }

    public async Task<SdkSessionInfo> ToSessionInfoAsync(SessionInfo sessionInfo, CancellationToken cancellationToken = new())
    {
        var client = await _clientRepository.GetAsync(sessionInfo.ClientId, cancellationToken);
        return new SdkSessionInfo
        {
            Id = sessionInfo.Id,
            Grant = sessionInfo.Grant.ToString()?.ToLower(),
            Client = ToClient(client),
            CreatedAt = sessionInfo.CreatedAt,
            ExpiresAt = sessionInfo.ExpiresAt,
            IssuedAt = sessionInfo.IssuedAt,
            UserAgent = sessionInfo.UserAgent is not null ? ToUserAgentInfo(sessionInfo.UserAgent) : null
        };
    }

    private SdkUserAgentInfo ToUserAgentInfo(UserAgentInfo userAgent)
    {
        return new SdkUserAgentInfo
        {
            Brand = userAgent.Brand,
            Device = userAgent.Device,
            Model = userAgent.Model,
            Client = userAgent.Client is not null ? ToClientInfo(userAgent.Client) : null,
            OperationSystem = userAgent.OperationSystem is not null ? ToOperationSystem(userAgent.OperationSystem) : null
        };
    }

    private SdkIClientInfo ToClientInfo(IClientInfo clientInfo)
    {
        if (clientInfo is BotClientInfo bot)
        {
            return new SdkBotClientInfo
            {
                Name = bot.ClientString
            };
        }

        if (clientInfo is BrowserClientInfo browser)
        {
            return new SdkBrowserClientInfo
            {
                Name = browser.Name,
                Version = browser.Version,
                ShortName = browser.ShortName,
                Engine = browser.Engine,
                EngineVersion = browser.EngineVersion
            };
        }

        throw new ArgumentException($"Convertor for client info of type {clientInfo.GetType()} is not implemented.");
    }

    private SdkOperationSystemInfo ToOperationSystem(OperationSystemInfo os)
    {
        return new SdkOperationSystemInfo
        {
            Name = os.Name,
            Platform = os.Platform,
            Version = os.Version,
            ShortName = os.ShortName
        };
    }

    public SdkClient ToClient(Client client)
    {
        return new SdkClient
        {
            Id = client.Id,
            ClientId = client.ClientId,
            ApplicationName = client.ApplicationName
        };
    }
}