using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Authentication.WebApi.Domain.Authentication;
using Qweree.Mongo;
using Qweree.Mongo.Exception;

namespace Qweree.Authentication.WebApi.Infrastructure.Authentication;

public class RefreshTokenRepository : MongoRepositoryBase<RefreshToken, RefreshTokenDo>, IRefreshTokenRepository
{
    public RefreshTokenRepository(MongoContext context) : base("refresh_tokens", context)
    {
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
}