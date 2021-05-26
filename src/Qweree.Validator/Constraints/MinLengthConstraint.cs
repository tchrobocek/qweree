using System.Collections;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Validator.ModelValidation;
using Qweree.Validator.ModelValidation.Attributes;

namespace Qweree.Validator.Constraints
{
    public class MinLengthConstraintValidator : ConstraintValidatorBase<IEnumerable, MinLengthConstraint>
    {
        protected override Task ValidateAsync(ValidationContext<IEnumerable> context, MinLengthConstraint constraint,
            ValidationBuilder builder, CancellationToken cancellationToken = new())
        {
            if (context.Subject == null)
                return Task.CompletedTask;

            var count = context.Subject.Cast<object>().Count();

            if (count != 0 && count < constraint.MinLength)
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