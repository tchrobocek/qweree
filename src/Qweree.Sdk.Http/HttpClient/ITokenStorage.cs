using System.Threading;
using System.Threading.Tasks;

namespace Qweree.Sdk.Http.HttpClient
{
    public interface ITokenStorage
    {
        Task SetAccessTokenAsync(string token, CancellationToken cancellationToken = new());
        Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = new());
    }

    public class MemoryTokenStorage : ITokenStorage
    {
        private string _token = string.Empty;

        public Task SetAccessTokenAsync(string token, CancellationToken cancellationToken = new CancellationToken())
        {
            _token = token;
            return Task.CompletedTask;
        }

        public Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = new())
        {
            return Task.FromResult(_token);
        }
    }
}