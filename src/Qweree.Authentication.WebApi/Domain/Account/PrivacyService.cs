using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Qweree.AspNet.Application;
using Qweree.Authentication.Sdk.Session;
using Qweree.Authentication.WebApi.Domain.Authentication;

namespace Qweree.Authentication.WebApi.Domain.Account;

public class PrivacyService
{
    private readonly ISessionStorage _sessionStorage;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public PrivacyService(ISessionStorage sessionStorage, IRefreshTokenRepository refreshTokenRepository)
    {
        _sessionStorage = sessionStorage;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<CollectionResponse<Sdk.Account.DeviceInfo>> FindMyDevicesAsync(CancellationToken cancellationToken = new())
    {
        var items = await _refreshTokenRepository.FindValidForUser(_sessionStorage.Id, cancellationToken);
        var result = items.Where(r => r.Device != null)
            .Select(r => new Sdk.Account.DeviceInfo(r.Id, r.Device!.Client, r.Device.Os, r.Device.Device,
                r.Device.Brand, r.Device.Model));
        return Response.Ok(result);
    }
}