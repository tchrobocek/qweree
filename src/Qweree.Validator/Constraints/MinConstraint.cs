using System.Threading;
using System.Threading.Tasks;
using Qweree.Validator.ModelValidation;
using Qweree.Validator.ModelValidation.Attributes;

namespace Qweree.Validator.Constraints
{
    public class MinConstraintValidator : ConstraintValidatorBase<int, MinConstraint>
    {
        protected override Task ValidateAsync(ValidationContext<int> context, MinConstraint constraint,
            ValidationBuilder builder, CancellationToken cancellationToken = new CancellationToken())
        {
            if (context.Subject < constraint.Min)
                builder.AddError(context.Path, constraint.Message);

            return Task.CompletedTask;
        }
    }

    public class MinConstraint : Constraint<MinConstraintValidator>
    {
        public MinConstraint(int min, string message)
        {
            Min = min;
            Message = message;
        }

        public int Min { get; }
        public string Message { get; set; }
    }

    public class MinConstraintAttribute : ConstraintAttribute
    {
        public MinConstraintAttribute(int min)
        {
            Min = min;
            Message = $"Minimum value is {min}.";
        }

        public int Min { get; }
        public string Message { get; set; }

        public override IConstraint CreateConstraint()
        {
            return new MinConstraint(Min, Message);
        }
    }
}