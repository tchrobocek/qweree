using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Qweree.AspNet.Application;
using Qweree.Authentication.Sdk.Authentication;
using Qweree.Authentication.WebApi.Domain.Authentication;
using Qweree.Authentication.WebApi.Domain.Identity;
using Qweree.Authentication.WebApi.Test.Fixture;
using Qweree.Authentication.WebApi.Test.Fixture.Factories;
using Qweree.Mongo.Exception;
using Qweree.TestUtils;
using Xunit;
using PasswordGrantInput = Qweree.Authentication.WebApi.Domain.Authentication.PasswordGrantInput;
using RefreshTokenGrantInput = Qweree.Authentication.WebApi.Domain.Authentication.RefreshTokenGrantInput;

namespace Qweree.Authentication.WebApi.Test.Domain.Authentication
{
    [Trait("Category", "Unit test")]
    public class AuthenticationServiceTest
    {
        [Fact]
        public async Task TestAuthenticatePasswordAsync()
        {
            var user = UserFactory.CreateDefault();

            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(m => m.GetByUsernameAsync(user.Username, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();

            var dateTimeProvider = new StaticDateTimeProvider();
            var service = new AuthenticationService(userRepositoryMock.Object, refreshTokenRepositoryMock.Object, dateTimeProvider, new Random(),
                10, 10, Settings.Authentication.AccessTokenKey, Settings.Authentication.FileAccessTokenKey, 0);

            var input = new PasswordGrantInput(user.Username, UserFactory.Password);
            var response = await service.AuthenticateAsync(input);

            Assert.Equal(ResponseStatus.Ok, response.Status);
            var tokenInfo = response.Payload ?? throw new ArgumentNullException();

            Assert.NotEmpty(tokenInfo.AccessToken);
            Assert.NotEmpty(tokenInfo.RefreshToken ?? "");
            Assert.Equal(dateTimeProvider.UtcNow + TimeSpan.FromSeconds(10), tokenInfo.ExpiresAt);
        }

        [Fact]
        public async Task TestAuthenticateRefreshTokenAsync()
        {
            var dateTimeProvider = new StaticDateTimeProvider();

            var user = UserFactory.CreateDefault();

            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(m => m.GetByUsernameAsync(user.Username, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            userRepositoryMock.Setup(m => m.GetAsync(user.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            RefreshToken? refreshToken = null;

            var refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
            refreshTokenRepositoryMock.Setup(m => m.InsertAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()))
                .Returns<RefreshToken, CancellationToken>((rt, _) =>
                {
                    Assert.Equal(user.Id, rt.UserId);
                    Assert.NotEqual(Guid.Empty, rt.Id);
                    Assert.NotEmpty(rt.Token);
                    Assert.Equal(dateTimeProvider.UtcNow + TimeSpan.FromSeconds(10), rt.ExpiresAt);
                    Assert.Equal(dateTimeProvider.UtcNow, rt.CreatedAt);

                    refreshToken = rt;
                    return Task.CompletedTask;
                });
            refreshTokenRepositoryMock.Setup(m => m.GetByTokenAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns<string, CancellationToken>((t, _) =>
                {
                    if (refreshToken == null)
                        throw new DocumentNotFoundException();

                    if (t != refreshToken.Token)
                        throw new DocumentNotFoundException();

                    return Task.FromResult(refreshToken);
                });

            var service = new AuthenticationService(userRepositoryMock.Object, refreshTokenRepositoryMock.Object, dateTimeProvider, new Random(),
                10, 10, Settings.Authentication.AccessTokenKey, Settings.Authentication.FileAccessTokenKey, 0);

            TokenInfo accessToken;

            {
                var input = new PasswordGrantInput(user.Username, UserFactory.Password);
                var response = await service.AuthenticateAsync(input);

                Assert.Equal(ResponseStatus.Ok, response.Status);
                accessToken = response.Payload ?? throw new ArgumentNullException();
            }

            {
                var input = new RefreshTokenGrantInput(accessToken.RefreshToken ?? "");
                var response = await service.AuthenticateAsync(input);
                var tokenInfo = response.Payload ?? throw new ArgumentNullException();

                Assert.Equal(ResponseStatus.Ok, response.Status);

                Assert.Equal(accessToken.RefreshToken, tokenInfo.RefreshToken);
                Assert.NotEmpty(tokenInfo.AccessToken);
                Assert.NotEqual(accessToken.AccessToken, tokenInfo.AccessToken);
                Assert.NotEmpty(tokenInfo.RefreshToken ?? "");
                Assert.Equal(dateTimeProvider.UtcNow + TimeSpan.FromSeconds(10), tokenInfo.ExpiresAt);
            }
        }

        [Fact]
        public async Task TestAuthenticateFileAccessTokenAsync()
        {
            var user = UserFactory.CreateDefault();

            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(m => m.GetByUsernameAsync(user.Username, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();

            var dateTimeProvider = new StaticDateTimeProvider();
            var service = new AuthenticationService(userRepositoryMock.Object, refreshTokenRepositoryMock.Object, dateTimeProvider, new Random(),
                10, 10, Settings.Authentication.AccessTokenKey, Settings.Authentication.FileAccessTokenKey, 10);

            TokenInfo accessToken;

            {
                var input = new PasswordGrantInput(user.Username, UserFactory.Password);
                var response = await service.AuthenticateAsync(input);

                Assert.Equal(ResponseStatus.Ok, response.Status);
                accessToken = response.Payload ?? throw new ArgumentNullException();
            }

            {
                var input = new FileAccessGrantInput(accessToken.AccessToken);
                var response = await service.AuthenticateAsync(input);

                Assert.Equal(ResponseStatus.Ok, response.Status);
                var tokenInfo = response.Payload ?? throw new ArgumentNullException();

                Assert.Null(tokenInfo.RefreshToken);
                Assert.NotEmpty(tokenInfo.AccessToken);
                Assert.NotEqual(accessToken.AccessToken, tokenInfo.AccessToken);
                Assert.Equal(dateTimeProvider.UtcNow + TimeSpan.FromSeconds(10), tokenInfo.ExpiresAt);
            }
        }
    }
}