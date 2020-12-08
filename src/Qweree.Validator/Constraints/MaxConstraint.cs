using System.Threading;
using System.Threading.Tasks;
using Qweree.Validator.ModelValidation;
using Qweree.Validator.ModelValidation.Attributes;

namespace Qweree.Validator.Constraints
{
    public class MaxConstraintValidator : ConstraintValidatorBase<int, MaxConstraint>
    {
        protected override Task ValidateAsync(ValidationContext<int> context, MaxConstraint constraint,
            ValidationBuilder builder, CancellationToken cancellationToken = new CancellationToken())
        {
            if (context.Subject > constraint.Max)
                builder.AddError(context.Path, constraint.Message);

            return Task.CompletedTask;
        }
    }

    public class MaxConstraint : Constraint<MaxConstraintValidator>
    {
        public MaxConstraint(int max, string message)
        {
            Max = max;
            Message = message;
        }

        public int Max { get; }
        public string Message { get; }
    }

    public class MaxConstraintAttribute : ConstraintAttribute
    {
        public MaxConstraintAttribute(int max)
        {
            Max = max;
            Message = $"Maximum value is {max}.";
        }

        public int Max { get; }
        public string Message { get; set; }

        public override IConstraint CreateConstraint()
        {
            return new MaxConstraint(Max, Message);
        }
    }
}