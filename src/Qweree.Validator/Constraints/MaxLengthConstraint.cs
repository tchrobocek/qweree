using System.Collections;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Validator.ModelValidation;
using Qweree.Validator.ModelValidation.Attributes;

namespace Qweree.Validator.Constraints;

public class MaxLengthConstraintValidator : ConstraintValidatorBase<IEnumerable, MaxLengthConstraint>
{
    protected override Task ValidateAsync(ValidationContext<IEnumerable> context, MaxLengthConstraint constraint,
        ValidationBuilder builder, CancellationToken cancellationToken = new())
    {
        if (context.Subject is null)
            return Task.CompletedTask;

        var count = context.Subject.Cast<object>().Count();

        if (count != 0 && count > constraint.MaxLength)
            builder.AddError(context.Path, constraint.Message);

        return Task.CompletedTask;
    }
}

public class MaxLengthConstraint : Constraint<MaxLengthConstraintValidator>
{
    public MaxLengthConstraint(int maxLength, string message)
    {
        MaxLength = maxLength;
        Message = message;
    }

    public int MaxLength { get; }
    public string Message { get; set; }
}

public class MaxLengthConstraintAttribute : ConstraintAttribute
{
    public MaxLengthConstraintAttribute(int maxLength)
    {
        MaxLength = maxLength;
        Message = $"Maximum length is {maxLength}.";
    }

    public int MaxLength { get; }
    public string Message { get; set; }

    public override IConstraint CreateConstraint()
    {
        return new MaxLengthConstraint(MaxLength, Message);
    }
}