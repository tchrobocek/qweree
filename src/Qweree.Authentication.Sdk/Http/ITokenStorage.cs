using System.Threading;
using System.Threading.Tasks;

namespace Qweree.Authentication.Sdk.Http;

public interface ITokenStorage
{
    Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = new());
}

public class MemoryTokenStorage : ITokenStorage
{
    private string _token = string.Empty;

    public Task SetAccessTokenAsync(string token, CancellationToken cancellationToken = new())
    {
        _token = token;
        return Task.CompletedTask;
    }

    public Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = new())
    {
        return Task.FromResult(_token);
    }
}