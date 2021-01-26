using System.Threading;
using System.Threading.Tasks;
using Qweree.Validator.ModelValidation;
using Qweree.Validator.ModelValidation.Attributes;

namespace Qweree.Validator.Constraints
{
    public class MinLengthConstraintValidator : ConstraintValidatorBase<string, MinLengthConstraint>
    {
        protected override Task ValidateAsync(ValidationContext<string> context, MinLengthConstraint constraint,
            ValidationBuilder builder, CancellationToken cancellationToken = new())
        {
            if (context.Subject.Length < constraint.MinLength)
                builder.AddError(context.Path, constraint.Message);

            return Task.CompletedTask;
        }
    }

    public class MinLengthConstraint : Constraint<MinLengthConstraintValidator>
    {
        public MinLengthConstraint(int minLength, string message)
        {
            MinLength = minLength;
            Message = message;
        }

        public int MinLength { get; }
        public string Message { get; }
    }

    public class MinLengthConstraintAttribute : ConstraintAttribute
    {
        public MinLengthConstraintAttribute(int minLength)
        {
            MinLength = minLength;
            Message = $"Minimum length is {minLength}.";
        }

        public int MinLength { get; }
        public string Message { get; set; }

        public override IConstraint CreateConstraint()
        {
            return new MinLengthConstraint(MinLength, Message);
        }
    }
}