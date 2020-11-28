using System;

namespace Qweree.Authentication.Sdk.Authentication
{
    public class TokenInfoMapper
    {
        public static TokenInfoDto ToDto(TokenInfo tokenInfo)
        {
            return new TokenInfoDto
            {
                AccessToken = tokenInfo.AccessToken,
                ExpiresAt = tokenInfo.ExpiresAt,
                RefreshToken = tokenInfo.RefreshToken
            };
        }

        public static TokenInfo FromDto(TokenInfoDto tokenInfoDto)
        {
            return new TokenInfo(tokenInfoDto.AccessToken ?? "", tokenInfoDto.RefreshToken, tokenInfoDto.ExpiresAt ?? DateTime.MinValue);
        }
    }
}