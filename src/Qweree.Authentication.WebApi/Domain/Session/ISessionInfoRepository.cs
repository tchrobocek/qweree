using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Qweree.Authentication.WebApi.Domain.Session;

public interface ISessionInfoRepository
{
    Task<SessionInfo> GetByRefreshTokenAsync(string token, CancellationToken cancellationToken = new());
    Task InsertAsync(SessionInfo refreshToken, CancellationToken cancellationToken = new());
    Task DeleteOneAsync(Guid id, CancellationToken cancellationToken = new());
    Task<IEnumerable<SessionInfo>> FindForUser(Guid userId, CancellationToken cancellationToken = new());
}