using System;
using System.Collections.Immutable;
using System.Linq;
using Qweree.Authentication.WebApi.Domain.Authentication;
using Qweree.Authentication.WebApi.Domain.Session;

namespace Qweree.Authentication.WebApi.Infrastructure.Session;

public class SessionInfoMapper
{
    public static SessionInfoDo ToDo(SessionInfo sessionInfo)
    {
        return new SessionInfoDo
        {
            Id = sessionInfo.Id,
            UserId = sessionInfo.UserId,
            Grant = sessionInfo.Grant.Key,
            ClientId = sessionInfo.ClientId,
            IpAddress = sessionInfo.IpAddress,
            UserAgent = sessionInfo.UserAgent is not null ? UserAgentInfoMapper.ToDo(sessionInfo.UserAgent) : null,
            CreatedAt = sessionInfo.CreatedAt,
            ExpiresAt = sessionInfo.ExpiresAt,
            IssuedAt = sessionInfo.IssuedAt,
            RefreshToken = sessionInfo.RefreshToken
        };
    }

    public static SessionInfo FromDo(SessionInfoDo sessionInfo)
    {
        return new SessionInfo(sessionInfo.Id ?? Guid.Empty,
            sessionInfo.ClientId ?? Guid.Empty,
            sessionInfo.UserId,
            sessionInfo.RefreshToken ?? string.Empty,
            sessionInfo.IpAddress ?? string.Empty,
            sessionInfo.UserAgent is not null ? UserAgentInfoMapper.FromDo(sessionInfo.UserAgent) : null,
            sessionInfo.Grant is not null ? FromKey(sessionInfo.Grant) : new GrantType(),
            sessionInfo.CreatedAt ?? DateTime.MinValue,
            sessionInfo.IssuedAt ?? DateTime.MinValue,
            sessionInfo.ExpiresAt ?? DateTime.MinValue);
    }

    public static readonly ImmutableArray<GrantType> GrantTypes = new[] { GrantType.Password, GrantType.RefreshToken, GrantType.ClientCredentials }.ToImmutableArray();

    public static GrantType FromKey(string key)
    {
        return GrantTypes.Single(g => g.Key == key);
    }

}