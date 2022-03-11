using System;
using Qweree.Authentication.WebApi.Domain.Authentication;

namespace Qweree.Authentication.WebApi.Infrastructure.Authentication;

public static class RefreshTokenMapper
{
    public static RefreshTokenDo ToDo(RefreshToken refreshToken)
    {
        return new RefreshTokenDo
        {
            Id = refreshToken.Id,
            Token = refreshToken.Token,
            CreatedAt = refreshToken.CreatedAt,
            ExpiresAt = refreshToken.ExpiresAt,
            UserId = refreshToken.UserId,
            ClientId = refreshToken.ClientId,
            Device = refreshToken.Device != null ? ToDo(refreshToken.Device) : null
        };
    }

    public static RefreshToken FromDo(RefreshTokenDo refreshToken)
    {
        return new RefreshToken(refreshToken.Id ?? Guid.Empty, refreshToken.Token ?? "", refreshToken.ClientId ?? Guid.Empty, refreshToken.UserId ?? Guid.Empty,
            refreshToken.ExpiresAt ?? DateTime.MinValue, refreshToken.CreatedAt ?? DateTime.MinValue, refreshToken.Device != null ? FromDo(refreshToken.Device) : null);
    }
    public static DeviceInfoDo ToDo(DeviceInfo deviceInfo)
    {
        return new DeviceInfoDo
        {
            Brand = deviceInfo.Brand,
            Client = deviceInfo.Client,
            Device = deviceInfo.Device,
            Model = deviceInfo.Model,
            Os = deviceInfo.Os,
        };
    }

    public static DeviceInfo FromDo(DeviceInfoDo deviceInfo)
    {
        return new DeviceInfo(deviceInfo.Client ?? string.Empty, deviceInfo.Os ?? string.Empty,
            deviceInfo.Device ?? string.Empty, deviceInfo.Brand ?? string.Empty, deviceInfo.Model ?? string.Empty);
    }
}