using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Validator.ModelValidation;
using Qweree.Validator.ModelValidation.Attributes;

namespace Qweree.Validator.Constraints
{
    public class OneOfConstraintValidator : ConstraintValidatorBase<object, OneOfConstraint>
    {
        protected override Task ValidateAsync(ValidationContext<object> context, OneOfConstraint constraint,
            ValidationBuilder builder, CancellationToken cancellationToken = new())
        {
            if (context.Subject == null)
                return Task.CompletedTask;

            if (!constraint.OneOf.Contains(context.Subject))
                builder.AddError(context.Path, constraint.Message);

            return Task.CompletedTask;
        }
    }

    public class OneOfConstraint : Constraint<OneOfConstraintValidator>
    {
        public OneOfConstraint(ImmutableArray<object> oneOf, string message)
        {
            OneOf = oneOf;
            Message = message;
        }

        public ImmutableArray<object> OneOf { get; }
        public string Message { get; }
    }

    public class OneOfConstraintAttribute : ConstraintAttribute
    {
        public OneOfConstraintAttribute(ImmutableArray<object> oneOf)
        {
            OneOf = oneOf;
            Message = $"Item must be one of [{string.Join(", ", oneOf.Select(o => o.ToString()))}].";
        }

        public ImmutableArray<object> OneOf { get; }
        public string Message { get; set; }

        public override IConstraint CreateConstraint()
        {
            return new OneOfConstraint(OneOf, Message);
        }
    }
}