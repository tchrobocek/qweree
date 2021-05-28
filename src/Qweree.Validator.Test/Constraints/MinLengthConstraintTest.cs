using System.Threading.Tasks;
using Qweree.Validator.Constraints;
using Xunit;

namespace Qweree.Validator.Test.Constraints
{
    public class MinLengthConstraintTest
    {
        [Fact]
        public async Task TestValidate()
        {
            var validator = new MinLengthConstraintValidator();
            var builder = new ValidationBuilder();
            await validator.ValidateAsync(new ValidationContext("", "a", null), new MinLengthConstraint(2, ""), builder);

            Assert.Single(builder.Build().Errors);
        }
    }
}