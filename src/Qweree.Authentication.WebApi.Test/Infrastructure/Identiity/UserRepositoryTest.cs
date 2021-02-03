using System.Threading.Tasks;
using DeepEqual.Syntax;
using Qweree.Authentication.WebApi.Infrastructure.Identity;
using Qweree.Authentication.WebApi.Test.Fixture;
using Qweree.Authentication.WebApi.Test.Fixture.Factories;
using Qweree.Mongo;
using Qweree.TestUtils.DeepEqual;
using Xunit;

namespace Qweree.Authentication.WebApi.Test.Infrastructure.Identiity
{
    [Collection("Database collection")]
    [Trait("Category", "Integration test")]
    [Trait("Category", "Database test")]
    public class UserRepositoryTest
    {
        private readonly UserRepository _repository;

        public UserRepositoryTest()
        {
            var context = new MongoContext(Settings.Database.ConnectionString, Settings.Database.DatabaseName);
            _repository = new UserRepository(context);
            _repository.DeleteAllAsync()
                .GetAwaiter()
                .GetResult();
        }

        [Fact]
        public async Task TestGetByUsername()
        {
            var user = UserFactory.CreateDefault();
            await _repository.InsertAsync(user);

            var actualUser = await _repository.GetByUsernameAsync(user.Username);

            actualUser.WithDeepEqual(user)
                .WithCustomComparison(new MillisecondDateTimeComparison())
                .Assert();
        }
    }
}