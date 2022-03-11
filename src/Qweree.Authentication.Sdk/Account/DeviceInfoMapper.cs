using System;

namespace Qweree.Authentication.Sdk.Account;

public static class DeviceInfoMapper
{
    public static DeviceInfoDto ToDto(DeviceInfo deviceInfo)
    {
        return new DeviceInfoDto
        {
            Brand = deviceInfo.Brand,
            Client = deviceInfo.Client,
            Device = deviceInfo.Device,
            Id = deviceInfo.Id,
            Model = deviceInfo.Model,
            Os = deviceInfo.Os
        };
    }

    public static DeviceInfo FromDto(DeviceInfoDto deviceInfo)
    {
        return new DeviceInfo(
            deviceInfo.Id ?? Guid.Empty,
            deviceInfo.Client ?? string.Empty,
            deviceInfo.Os ?? string.Empty,
            deviceInfo.Device ?? string.Empty,
            deviceInfo.Brand ?? string.Empty,
            deviceInfo.Model ?? string.Empty);
    }
}