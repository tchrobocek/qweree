using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Qweree.Authentication.WebApi.Domain.Session;

public interface ISessionInfoRepository
{
    Task<SessionInfo> GetByRefreshTokenAsync(string token, CancellationToken cancellationToken = new());
    Task InsertAsync(SessionInfo sessionInfo, CancellationToken cancellationToken = new());
    Task ReplaceAsync(Guid id, SessionInfo sessionInfo, CancellationToken cancellationToken = new());
    Task<SessionInfo> GetAsync(Guid id, CancellationToken cancellationToken = new());
    Task DeleteOneAsync(Guid id, CancellationToken cancellationToken = new());
    Task<IEnumerable<SessionInfo>> FindActiveSessionsForUser(Guid userId, CancellationToken cancellationToken = new());
    Task<IEnumerable<SessionInfo>> FindActiveSessionsForClient(Guid clientId, CancellationToken cancellationToken = new());
}