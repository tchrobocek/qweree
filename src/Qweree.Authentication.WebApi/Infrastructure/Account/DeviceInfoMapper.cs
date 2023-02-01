using Qweree.Authentication.WebApi.Domain.Account;
using SdkDeviceInfo = Qweree.Authentication.Sdk.Account.DeviceInfo;

namespace Qweree.Authentication.WebApi.Infrastructure.Account;

public static class DeviceInfoMapper
{
    public static SdkDeviceInfo Map(DeviceInfo deviceInfo)
    {
        return new SdkDeviceInfo
        {
            Brand = deviceInfo.Brand,
            Client = deviceInfo.Client,
            Device = deviceInfo.Device,
            Id = deviceInfo.Id,
            Model = deviceInfo.Model,
            Os = deviceInfo.Os,
            IssuedAt = deviceInfo.IssuedAt,
            ExpiresAt = deviceInfo.ExpiresAt
        };
    }
}