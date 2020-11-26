using System.Threading.Tasks;
using Qweree.Validator.Constraints;
using Xunit;

namespace Qweree.Validator.Test.Constraints
{
    public class MinConstraintTest
    {
        [Fact]
        public async Task TestValidate()
        {
            var validator = new MinConstraintValidator();
            var builder = new ValidationBuilder();
            await validator.ValidateAsync(new ValidationContext("", 0), new MinConstraint(1, ""), builder);

            Assert.Single(builder.Build().Errors);
        }
    }
}