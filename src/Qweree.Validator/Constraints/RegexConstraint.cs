using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Validator.ModelValidation;
using Qweree.Validator.ModelValidation.Attributes;

namespace Qweree.Validator.Constraints
{
    public class RegexConstraintValidator : ConstraintValidatorBase<string, RegexConstraint>
    {
        protected override Task ValidateAsync(ValidationContext<string> context, RegexConstraint constraint,
            ValidationBuilder builder, CancellationToken cancellationToken = new())
        {
            if (context.Subject == null)
                return Task.CompletedTask;

            if (!constraint.Regex.IsMatch(context.Subject))
                builder.AddError(context.Path, constraint.Message);

            return Task.CompletedTask;
        }
    }

    public class RegexConstraint : Constraint<RegexConstraintValidator>
    {
        public RegexConstraint(Regex regex, string message)
        {
            Regex = regex;
            Message = message;
        }

        public Regex Regex { get; }
        public string Message { get; }
    }

    public class RegexConstraintAttribute : ConstraintAttribute
    {
        public RegexConstraintAttribute(Regex regex)
        {
            Regex = regex;
            Message = $@"Value does not match ""{regex}"".";
        }

        public Regex Regex { get; }
        public string Message { get; set; }

        public override IConstraint CreateConstraint()
        {
            return new RegexConstraint(Regex, Message);
        }
    }
}