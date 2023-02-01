using System;
using Qweree.Authentication.WebApi.Domain.Session;

namespace Qweree.Authentication.WebApi.Infrastructure.Session;

public abstract class UserAgentInfoMapper
{
    public static UserAgentInfo FromDo(UserAgentInfoDo userAgent)
    {
        return new UserAgentInfo(userAgent.UserAgentString ?? string.Empty,
            FromDo(userAgent.Client!),
            userAgent.OperationSystem is not null ? FromDo(userAgent.OperationSystem) : null,
            userAgent.Device ?? string.Empty,
            userAgent.Brand ?? string.Empty,
            userAgent.Model ?? string.Empty
        );
    }

    public static OperationSystemInfo FromDo(OperationSystemInfoDo os)
    {
        return new OperationSystemInfo(os.OperationSystemString ?? string.Empty,
            os.Name ?? string.Empty,
            os.ShortName ?? string.Empty,
            os.Version ?? string.Empty,
            os.Platform ?? string.Empty);
    }

    public static IClientInfo FromDo(IClientInfoDo client)
    {
        if (client is BotClientInfoDo)
        {
            return new BotClientInfo(client.ClientString ?? string.Empty);
        }

        if (client is BrowserClientInfoDo browser)
        {
            return new BrowserClientInfo(browser.ClientString ?? string.Empty,
                browser.Name ?? string.Empty,
                browser.Version ?? string.Empty,
                browser.ShortName ?? string.Empty,
                browser.Engine ?? string.Empty,
                browser.EngineVersion ?? string.Empty);
        }

        throw new ArgumentException($"Conversion for browser type of {client.GetType()} is not implemented.");
    }

    public static UserAgentInfoDo ToDo(UserAgentInfo userAgent)
    {
        return new UserAgentInfoDo
        {
            UserAgentString = userAgent.UserAgentString,
            Client = userAgent.Client is not null ? ToDo(userAgent.Client) : null,
            OperationSystem = userAgent.OperationSystem is not null ? ToDo(userAgent.OperationSystem) : null,
            Brand = userAgent.Brand,
            Device = userAgent.Device,
            Model = userAgent.Model
        };
    }

    public static IClientInfoDo ToDo(IClientInfo client)
    {
        if (client is BotClientInfo)
        {
            return new BotClientInfoDo
            {
                ClientString = client.ClientString
            };
        }
        if (client is BrowserClientInfo browser)
        {
            return new BrowserClientInfoDo
            {
                ClientString = client.ClientString,
                Engine = browser.Engine,
                EngineVersion = browser.EngineVersion,
                Name = browser.Name,
                ShortName = browser.ShortName,
                Version = browser.Version,
            };
        }

        throw new ArgumentException($"Conversion for browser type of {client.GetType()} is not implemented.");
    }

    public static OperationSystemInfoDo ToDo(OperationSystemInfo os)
    {
        return new OperationSystemInfoDo
        {
            OperationSystemString = os.OperationSystemString,
            Name = os.Name,
            Platform = os.Platform,
            ShortName = os.ShortName,
            Version = os.Version
        };
    }
}