using Qweree.Authentication.WebApi.Domain.Authentication;

namespace Qweree.Authentication.WebApi.Infrastructure.Authentication;

public class DeviceInfoMapper
{
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