using System.Threading.Tasks;
using Qweree.Validator.Constraints;
using Xunit;

namespace Qweree.Validator.Test.Constraints
{
    public class MaxLengthConstraintTest
    {
        [Fact]
        public async Task TestValidate()
        {
            var validator = new MaxLengthConstraintValidator();
            var builder = new ValidationBuilder();
            await validator.ValidateAsync(new ValidationContext("", "a"), new MaxLengthConstraint(0, ""), builder);

            Assert.Single(builder.Build().Errors);
        }
    }
}