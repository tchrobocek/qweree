using System.Collections;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Validator.ModelValidation;
using Qweree.Validator.ModelValidation.Attributes;

namespace Qweree.Validator.Constraints
{
    public class NotEmptyConstraintValidator : ConstraintValidatorBase<IEnumerable, NotEmptyConstraint>
    {
        protected override Task ValidateAsync(ValidationContext<IEnumerable> context, NotEmptyConstraint constraint,
            ValidationBuilder builder, CancellationToken cancellationToken = new())
        {
            if (context.Subject == null)
                return Task.CompletedTask;

            if (!context.Subject.Cast<object>().Any())
                builder.AddError(context.Path, constraint.Message);

            return Task.CompletedTask;
        }
    }

    public class NotEmptyConstraint : Constraint<NotEmptyConstraintValidator>
    {
        public NotEmptyConstraint(string message)
        {
            Message = message;
        }

        public string Message { get; set; }
    }

    public class NotEmptyConstraintAttribute : ConstraintAttribute
    {
        public NotEmptyConstraintAttribute()
        {
            Message = "Cannot be empty.";
        }

        public string Message { get; set; }

        public override IConstraint CreateConstraint()
        {
            return new NotEmptyConstraint(Message);
        }
    }
}