using System;
using Qweree.Authentication.WebApi.Domain.Authentication;

namespace Qweree.Authentication.WebApi.Infrastructure.Authentication;

public static class RefreshTokenMapper
{
    public static RefreshTokenDo ToDo(RefreshToken refreshToken)
    {
        return new RefreshTokenDo
        {
            Id = refreshToken.Id,
            Token = refreshToken.Token,
            CreatedAt = refreshToken.CreatedAt,
            ExpiresAt = refreshToken.ExpiresAt,
            UserId = refreshToken.UserId,
            ClientId = refreshToken.ClientId
        };
    }

    public static RefreshToken FromDo(RefreshTokenDo refreshToken)
    {
        return new RefreshToken(refreshToken.Id ?? Guid.Empty, refreshToken.Token ?? "", refreshToken.ClientId ?? Guid.Empty, refreshToken.UserId ?? Guid.Empty,
            refreshToken.ExpiresAt ?? DateTime.MinValue, refreshToken.CreatedAt ?? DateTime.MinValue);
    }
}