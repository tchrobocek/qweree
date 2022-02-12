using System;
using System.Threading;
using System.Threading.Tasks;

namespace Qweree.Cdn.WebApi.Domain.Storage;

public interface IBufferItem : IDisposable, IAsyncDisposable
{
    long Length { get; }
    Task StoreAsync(string targetPath, CancellationToken cancellationToken);
}