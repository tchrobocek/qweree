using System.Threading.Tasks;
using Qweree.Validator.Constraints;
using Xunit;

namespace Qweree.Validator.Test.Constraints;

public class EmailConstraintTest
{
    [Fact]
    public async Task TestValidate_Format()
    {
        var validator = new EmailConstraintValidator();
        var builder = new ValidationBuilder();
        await validator.ValidateAsync(new ValidationContext("b", "asdf", null), new EmailConstraint("", ""), builder);

        Assert.Single(builder.Build().Errors);
    }
}