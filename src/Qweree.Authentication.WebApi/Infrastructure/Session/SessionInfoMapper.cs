using System;
using Qweree.Authentication.WebApi.Domain.Authentication;
using Qweree.Authentication.WebApi.Domain.Session;
using DeviceInfoMapper = Qweree.Authentication.WebApi.Infrastructure.Authentication.DeviceInfoMapper;

namespace Qweree.Authentication.WebApi.Infrastructure.Session;

public class SessionInfoMapper
{
    public static SessionInfoDo ToDo(SessionInfo sessionInfo)
    {
        return new SessionInfoDo
        {
            Id = sessionInfo.Id,
            UserId = sessionInfo.UserId,
            Grant = sessionInfo.Grant,
            ClientId = sessionInfo.ClientId,
            Device = sessionInfo.Device != null ? DeviceInfoMapper.ToDo(sessionInfo.Device) : null,
            CreatedAt = sessionInfo.CreatedAt,
            ExpiresAt = sessionInfo.ExpiresAt,
            IssuedAt = sessionInfo.IssuedAt,
            RefreshToken = sessionInfo.RefreshToken
        };
    }

    public static SessionInfo FromDo(SessionInfoDo sessionInfo)
    {
        return new SessionInfo(sessionInfo.Id ?? Guid.Empty, sessionInfo.ClientId ?? Guid.Empty, sessionInfo.UserId,
            sessionInfo.RefreshToken ?? string.Empty,
            sessionInfo.Device != null ? DeviceInfoMapper.FromDo(sessionInfo.Device) : null, sessionInfo.Grant ?? new GrantType(), sessionInfo.CreatedAt ?? DateTime.MinValue,
            sessionInfo.IssuedAt ?? DateTime.MinValue, sessionInfo.ExpiresAt ?? DateTime.MinValue);
    }

}