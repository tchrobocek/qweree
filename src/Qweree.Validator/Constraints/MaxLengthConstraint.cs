using System.Threading;
using System.Threading.Tasks;
using Qweree.Validator.ModelValidation;
using Qweree.Validator.ModelValidation.Attributes;

namespace Qweree.Validator.Constraints
{
    public class MaxLengthConstraintValidator : ConstraintValidatorBase<string, MaxLengthConstraint>
    {
        protected override Task ValidateAsync(ValidationContext<string> context, MaxLengthConstraint constraint,
            ValidationBuilder builder, CancellationToken cancellationToken = new CancellationToken())
        {
            if (context.Subject.Length > constraint.MaxLength)
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
}