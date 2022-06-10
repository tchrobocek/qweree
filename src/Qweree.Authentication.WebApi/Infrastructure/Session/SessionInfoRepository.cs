using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Authentication.WebApi.Domain.Session;
using Qweree.Mongo;
using Qweree.Mongo.Exception;
using Qweree.Utils;

namespace Qweree.Authentication.WebApi.Infrastructure.Session;

public class SessionInfoRepository : MongoRepositoryBase<SessionInfo,  SessionInfoDo>, ISessionInfoRepository
{
    private const string IsoDateFormat = "yyyy-MM-ddTHH:mm:ss";
    private readonly IDateTimeProvider _dateTimeProvider;

    public SessionInfoRepository(MongoContext context,
        IDateTimeProvider dateTimeProvider) : base("sessions", context)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    protected override Func<SessionInfo, SessionInfoDo> ToDocument => SessionInfoMapper.ToDo;
    protected override Func<SessionInfoDo, SessionInfo> FromDocument => SessionInfoMapper.FromDo;

    public async Task<SessionInfo> GetByRefreshTokenAsync(string token, CancellationToken cancellationToken = new())
    {
        var query = $@"{{""RefreshToken"": ""{token}""}}";
        var result = (await FindAsync(query, cancellationToken))
            .FirstOrDefault();

        if (result == null)
            throw new DocumentNotFoundException("Session does not exist.");

        return result;
    }

    public async Task<IEnumerable<SessionInfo>> FindForUser(Guid userId, CancellationToken cancellationToken = new())
    {
        var now = _dateTimeProvider.UtcNow;
        var query = $@"{{""$and"": [{{""UserId"": UUID(""{userId}"")}}, {{""ExpiresAt"": {{""$gte"": ISODate(""{now.ToString(IsoDateFormat)}"")}}}}]}}";
        return await FindAsync(query, cancellationToken);
    }
}