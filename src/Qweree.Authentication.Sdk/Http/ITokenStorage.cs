using System.Threading;
using System.Threading.Tasks;
using Qweree.Authentication.Sdk.OAuth2;

namespace Qweree.Authentication.Sdk.Http;

public interface ITokenStorage
{
    Task SetTokenInfoAsync(TokenInfo? tokenInfo, CancellationToken cancellationToken = new());
    Task<TokenInfo?> GetTokenInfoAsync(CancellationToken cancellationToken = new());
}

public class MemoryTokenStorage : ITokenStorage
{
    private TokenInfo? _tokenInfo;

    public Task SetTokenInfoAsync(TokenInfo? tokenInfo, CancellationToken cancellationToken = new())
    {
        _tokenInfo = tokenInfo;
        return Task.CompletedTask;
    }

    public Task<TokenInfo?> GetTokenInfoAsync(CancellationToken cancellationToken = new())
    {
        return Task.FromResult(_tokenInfo);
    }
}