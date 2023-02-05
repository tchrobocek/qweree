using System;
using System.Collections;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Validator.ModelValidation;
using Qweree.Validator.ModelValidation.Attributes;

namespace Qweree.Validator.Constraints;

public class MaxOfTypeConstraintValidator : ConstraintValidatorBase<IEnumerable, MaxOfTypeConstraint>
{
    protected override Task ValidateAsync(ValidationContext<IEnumerable> context, MaxOfTypeConstraint constraint,
        ValidationBuilder builder, CancellationToken cancellationToken = new())
    {
        if (context.Subject is null)
            return Task.CompletedTask;

        var count = context.Subject
            .Cast<object>()
            .Count(s => s.GetType() == constraint.Type);

        if (count != 0 && count > constraint.MaxLength)
            builder.AddError(context.Path, constraint.Message);

        return Task.CompletedTask;
    }
}

public class MaxOfTypeConstraint : Constraint<MaxOfTypeConstraintValidator>
{
    public MaxOfTypeConstraint(Type type, int maxLength, string message)
    {
        Type = type;
        MaxLength = maxLength;
        Message = message;
    }

    public Type Type { get; }
    public int MaxLength { get; }
    public string Message { get; set; }
}

public class MaxOfTypeConstraint<TSubjectType> : MaxOfTypeConstraint
{
    public MaxOfTypeConstraint(int maxLength, string message)
        :base(typeof(TSubjectType), maxLength, message)
    {
    }
}

public class MaxOfTypeConstraintAttribute : ConstraintAttribute
{
    public MaxOfTypeConstraintAttribute(Type type, int maxLength)
    {
        Type = type;
        MaxLength = maxLength;
        Message = $"Maximum length is {maxLength}.";
    }

    public Type Type { get; }
    public int MaxLength { get; }
    public string Message { get; set; }

    public override IConstraint CreateConstraint()
    {
        return new MaxOfTypeConstraint(Type, MaxLength, Message);
    }
}