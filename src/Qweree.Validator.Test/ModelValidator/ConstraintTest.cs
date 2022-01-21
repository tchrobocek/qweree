using System.Threading;
using System.Threading.Tasks;
using Qweree.Validator.ModelValidation;
using Xunit;

namespace Qweree.Validator.Test.ModelValidator;

public class ConstraintTest
{
    [Fact]
    public void TestGetValidator()
    {
        var constraint = new Constraint<NoneValidator>();
        Assert.Equal(typeof(NoneValidator), constraint.ValidatorType);
    }

    private class NoneValidator : IConstraintValidator
    {
        public Task ValidateAsync(ValidationContext context, IConstraint constraint, ValidationBuilder builder,
            CancellationToken cancellationToken = new())
        {
            return Task.CompletedTask;
        }
    }
}