using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Qweree.AspNet.Application;
using Qweree.Authentication.WebApi.Application.Identity;
using Qweree.Authentication.WebApi.Domain.Identity;
using Qweree.Mongo.Exception;
using Qweree.TestUtils;
using Qweree.TestUtils.Qweree.Validator;
using Qweree.Utils;
using Xunit;

namespace Qweree.Authentication.WebApi.Test.Application.Identity
{
    [Trait("Category", "Unit test")]
    public class UserServiceTest
    {
        [Fact]
        public async Task TestCreateUser()
        {
            var now = DateTime.UtcNow;

            var userRepositoryMock = new Mock<IUserRepository>();
            var service = new UserService(new StaticDateTimeProvider(now), userRepositoryMock.Object, new EmptyValidator());
            var input = new UserCreateInput("username", "email", "full name", "password", new[] {"Role"});
            var response = await service.CreateUserAsync(input);
            Assert.Equal(ResponseStatus.Ok, response.Status);
            var user = response.Payload;

            Assert.NotNull(user);
            Assert.Equal("username", user?.Username);
            Assert.Equal("email", user?.ContactEmail);
            Assert.Equal("full name", user?.FullName);
            Assert.NotEmpty(user?.Password ?? "");
            Assert.NotEqual("password", user?.Password);
            Assert.NotEqual(Guid.Empty, user?.Id);
            Assert.Equal(now, user?.CreatedAt);
            Assert.Equal(now, user?.ModifiedAt);
            Assert.Single(user?.Roles ?? ImmutableArray<string>.Empty);
            Assert.Single(user?.Roles ?? ImmutableArray<string>.Empty, "Role");
        }

        [Fact]
        public async Task TestCreateUser_RepositoryError()
        {
            var userRepositoryMock = new Mock<IUserRepository>();

            userRepositoryMock.Setup(m => m.InsertAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .Callback(() => throw new InsertDocumentException());

            var service = new UserService(new DateTimeProvider(), userRepositoryMock.Object, new EmptyValidator());
            var input = new UserCreateInput("username", "email", "full name", "password", Array.Empty<string>());
            var response = await service.CreateUserAsync(input);
            Assert.Equal(ResponseStatus.Fail, response.Status);
            Assert.Null(response.Payload);
            Assert.Single(response.Errors);
        }
    }
}