using System;
using System.Collections.Generic;
using System.Linq;
using DeviceDetectorNET;
using Qweree.Authentication.WebApi.Domain.Session;

namespace Qweree.Authentication.WebApi.Infrastructure.Session;

public static class UserAgentInfoParser
{
    public static UserAgentInfo? Parse(string userAgentString)
    {
        if (string.IsNullOrWhiteSpace(userAgentString))
            return null;

        var detector = new DeviceDetector(userAgentString);
        detector.Parse();

        if(detector.IsBot())
        {
            var result = detector.GetBot();

            if (result.Success)
                return new UserAgentInfo(userAgentString, new BotClientInfo(result.ToString()),
                    null, "bot", string.Empty, string.Empty);

            return null;
        }

        var detectedClient = detector.GetClient();
        var detectedOs = detector.GetOs();

        IClientInfo? client = null;
        OperationSystemInfo? os = null;
        if (detectedClient.Success)
            client = ParseClientInfo(detectedClient.ToString());
        if (detectedClient.Success)
            os = ParseOs(detectedOs.ToString());

        var device = detector.GetDeviceName();
        var brand  = detector.GetBrandName();
        var model  = detector.GetModel();

        return new UserAgentInfo(userAgentString, client, os, device, brand, model);
    }

    private static OperationSystemInfo ParseOs(string osString)
    {
        var values = ParseString(osString);
        values.TryGetValue("name", out var name);
        values.TryGetValue("version", out var version);
        values.TryGetValue("shortname", out var shortname);
        values.TryGetValue("platform", out var platform);
        return new OperationSystemInfo(osString, name ?? string.Empty, shortname ?? string.Empty, version ?? string.Empty,
            platform ?? string.Empty);
    }

    private static BrowserClientInfo ParseClientInfo(string clientString)
    {
        var values = ParseString(clientString);

        var type = values["type"];

        if (type != "browser")
            throw new ArgumentException($"Unknown type: {type}");

        values.TryGetValue("name", out var name);
        values.TryGetValue("version", out var version);
        values.TryGetValue("shortname", out var shortname);
        values.TryGetValue("engine", out var engine);
        values.TryGetValue("engineVersion", out var engineVersion);
        return new BrowserClientInfo(clientString, name ?? string.Empty, version ?? string.Empty, shortname ?? string.Empty,
            engine ?? string.Empty, engineVersion ?? string.Empty);
    }

    private static IDictionary<string, string> ParseString(string value)
    {
        return value.Split(';')
            .Select(v => v.Split(':', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
            .Where(v => v.Length == 2)
            .ToDictionary(v => v[0].ToLower(), v => v[1]);
    }
}