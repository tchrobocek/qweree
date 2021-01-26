using Qweree.Authentication.WebApi.Domain.Authentication;

namespace Qweree.Authentication.WebApi.Infrastructure.Authentication
{
    public static class RefreshTokenMapper
    {
        public static RefreshTokenDo ToDo(RefreshToken refreshToken)
        {
            return new()
            {
                Id = refreshToken.Id,
                Token = refreshToken.Token,
                CreatedAt = refreshToken.CreatedAt,
                ExpiresAt = refreshToken.ExpiresAt,
                UserId = refreshToken.UserId
            };
        }

        public static RefreshToken FromDo(RefreshTokenDo refreshToken)
        {
            return new(refreshToken.Id, refreshToken.Token ?? "", refreshToken.UserId, refreshToken.ExpiresAt,
                refreshToken.CreatedAt);
        }
    }
}