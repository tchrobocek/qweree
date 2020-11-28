using System.Threading.Tasks;
using Qweree.Validator.Constraints;
using Xunit;

namespace Qweree.Validator.Test.Constraints
{
    public class MaxConstraintTest
    {
        [Fact]
        public async Task TestValidate()
        {
            var validator = new MaxConstraintValidator();
            var builder = new ValidationBuilder();
            await validator.ValidateAsync(new ValidationContext("", 1, null), new MaxConstraint(0, ""), builder);

            Assert.Single(builder.Build().Errors);
        }
    }
}