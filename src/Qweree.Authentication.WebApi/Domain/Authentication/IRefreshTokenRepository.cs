using System;
using System.Threading;
using System.Threading.Tasks;

namespace Qweree.Authentication.WebApi.Domain.Authentication;

public interface IRefreshTokenRepository
{
    Task<RefreshToken> GetByTokenAsync(string token, CancellationToken cancellationToken = new());
    Task InsertAsync(RefreshToken refreshToken, CancellationToken cancellationToken = new());
    Task DeleteOneAsync(Guid id, CancellationToken cancellationToken = new());
}