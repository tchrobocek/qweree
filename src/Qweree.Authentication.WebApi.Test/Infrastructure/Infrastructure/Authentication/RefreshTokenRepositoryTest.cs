using System;
using System.Threading.Tasks;
using DeepEqual.Syntax;
using Qweree.Authentication.WebApi.Domain.Authentication;
using Qweree.Authentication.WebApi.Infrastructure.Authentication;
using Qweree.Authentication.WebApi.Test.Fixture;
using Qweree.Mongo;
using Qweree.TestUtils.DeepEqual;
using Xunit;

namespace Qweree.Authentication.WebApi.Test.Infrastructure.Infrastructure.Authentication
{
    [Collection("Database collection")]
    [Trait("Category", "Integration test")]
    [Trait("Category", "Database test")]
    public class RefreshTokenRepositoryTest
    {
        private readonly RefreshTokenRepository _repository;

        public RefreshTokenRepositoryTest()
        {
            var context = new MongoContext(Settings.Database.ConnectionString, Settings.Database.DatabaseName);
            _repository = new RefreshTokenRepository(context);
            _repository.DeleteAllAsync()
                .GetAwaiter()
                .GetResult();
        }

        [Fact]
        public async Task TestGetByToken()
        {
            var expectedToken = new RefreshToken(Guid.NewGuid(), "token", Guid.NewGuid(), DateTime.UtcNow, DateTime.UtcNow);
            await _repository.InsertAsync(expectedToken);

            var actualToken = await _repository.GetByTokenAsync(expectedToken.Token);

            actualToken.WithDeepEqual(expectedToken)
                .WithCustomComparison(new MillisecondDateTimeComparison())
                .Assert();
        }
    }
}