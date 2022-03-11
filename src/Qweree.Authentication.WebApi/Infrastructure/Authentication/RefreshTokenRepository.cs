using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Authentication.WebApi.Domain.Authentication;
using Qweree.Mongo;
using Qweree.Mongo.Exception;
using Qweree.Utils;

namespace Qweree.Authentication.WebApi.Infrastructure.Authentication;

public class RefreshTokenRepository : MongoRepositoryBase<RefreshToken, RefreshTokenDo>, IRefreshTokenRepository
{
    // ReSharper disable once InconsistentNaming
    private const string ISODateFormat = "yyyy-MM-ddTHH:mm:ss";
    private readonly IDateTimeProvider _dateTimeProvider;

    public RefreshTokenRepository(MongoContext context, IDateTimeProvider dateTimeProvider) : base("refresh_tokens", context)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    protected override Func<RefreshToken, RefreshTokenDo> ToDocument => RefreshTokenMapper.ToDo;
    protected override Func<RefreshTokenDo, RefreshToken> FromDocument => RefreshTokenMapper.FromDo;

    public async Task<RefreshToken> GetByTokenAsync(string token, CancellationToken cancellationToken = new())
    {
        var refreshToken = (await FindAsync($@"{{""Token"": ""{token}""}}", 0, 1, cancellationToken))
            .FirstOrDefault();

        if (refreshToken == null)
            throw new DocumentNotFoundException(@$"Refresh token ""{refreshToken}"" was not found.");

        return refreshToken;
    }

    public async Task<IEnumerable<RefreshToken>> FindValidForUser(Guid userId, CancellationToken cancellationToken = new())
    {
        var now = _dateTimeProvider.UtcNow;
        var query = $@"{{""$and"": [{{""UserId"": UUID(""{userId}"")}}, {{""ExpiresAt"": {{""$gte"": ISODate(""{now.ToString(ISODateFormat)}"")}}}}]}}";
        return await FindAsync(query, cancellationToken);
    }
}