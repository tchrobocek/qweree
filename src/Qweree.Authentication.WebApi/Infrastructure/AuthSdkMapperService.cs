using System;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Authentication.Sdk.Account.MyAccount;
using Qweree.Authentication.Sdk.Identity;
using Qweree.Authentication.WebApi.Domain.Identity;
using BotClientInfo = Qweree.Authentication.WebApi.Domain.Session.BotClientInfo;
using BrowserClientInfo = Qweree.Authentication.WebApi.Domain.Session.BrowserClientInfo;
using IClientInfo = Qweree.Authentication.WebApi.Domain.Session.IClientInfo;
using OperationSystemInfo = Qweree.Authentication.WebApi.Domain.Session.OperationSystemInfo;
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

    public async Task<MyAccountSessionInfo> ToSessionInfoAsync(SessionInfo sessionInfo, CancellationToken cancellationToken = new())
    {
        var client = await _clientRepository.GetAsync(sessionInfo.ClientId, cancellationToken);
        return new MyAccountSessionInfo
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

    private AuthUserAgentInfo ToUserAgentInfo(UserAgentInfo userAgent)
    {
        return new AuthUserAgentInfo
        {
            Brand = userAgent.Brand,
            Device = userAgent.Device,
            Model = userAgent.Model,
            Client = userAgent.Client is not null ? ToClientInfo(userAgent.Client) : null,
            OperationSystem = userAgent.OperationSystem is not null ? ToOperationSystem(userAgent.OperationSystem) : null
        };
    }

    private IAuthClientInfo ToClientInfo(IClientInfo clientInfo)
    {
        if (clientInfo is BotClientInfo bot)
        {
            return new BotAuthClientInfo
            {
                Name = bot.ClientString
            };
        }

        if (clientInfo is BrowserClientInfo browser)
        {
            return new BrowserAuthClientInfo
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

    private AuthOperationSystemInfo ToOperationSystem(OperationSystemInfo os)
    {
        return new AuthOperationSystemInfo
        {
            Name = os.Name,
            Platform = os.Platform,
            Version = os.Version,
            ShortName = os.ShortName
        };
    }

    public AuthClient ToClient(Client client)
    {
        return new AuthClient
        {
            Id = client.Id,
            ClientId = client.ClientId,
            ApplicationName = client.ApplicationName
        };
    }
}