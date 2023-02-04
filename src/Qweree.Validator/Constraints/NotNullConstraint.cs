using System.Threading;
using System.Threading.Tasks;
using Qweree.Validator.ModelValidation;
using Qweree.Validator.ModelValidation.Attributes;

namespace Qweree.Validator.Constraints;

public class NotNullConstraintValidator : ConstraintValidatorBase<object, NotNullConstraint>
{
    protected override Task ValidateAsync(ValidationContext<object> context, NotNullConstraint constraint,
        ValidationBuilder builder, CancellationToken cancellationToken = new())
    {
        if (context.Subject is null)
            builder.AddError(context.Path, constraint.NullMessage);

        return Task.CompletedTask;
    }
}

public class NotNullConstraint : Constraint<NotNullConstraintValidator>
{
    public NotNullConstraint(string nullMessage)
    {
        NullMessage = nullMessage;
    }

    public string NullMessage { get; }
}

public class NotNullConstraintAttribute : ConstraintAttribute
{
    public NotNullConstraintAttribute()
    {
        NullMessage = "Is null.";
    }

    public string NullMessage { get; set; }

    public override IConstraint CreateConstraint()
    {
        return new NotNullConstraint(NullMessage);
    }
}