using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Qweree.Validator.Constraints;
using Xunit;

namespace Qweree.Validator.Test.Constraints
{
    public class RegexConstraintTest
    {
        [Fact]
        public async Task TestValidate()
        {
            var validator = new RegexConstraintValidator();
            var builder = new ValidationBuilder();
            await validator.ValidateAsync(new ValidationContext("b", "", null), new RegexConstraint(new Regex("/a/"), ""),
                builder);

            Assert.Single(builder.Build().Errors);
        }
    }
}