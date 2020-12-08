using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Validator;
using Qweree.Validator.ModelValidation;

namespace Qweree.Authentication.WebApi.Infrastructure.Validations
{
    public class PasswordConstraintValidator : ConstraintValidatorBase<string, PasswordConstraint>
    {
        private static readonly Regex HasNumberRegex = new Regex(@"[0-9]+");
        private static readonly Regex HasUpperCaseRegex = new Regex(@"[A-Z]+");
        private static readonly Regex HasMinimum8CharsRegex = new Regex(@".{8,}");

        protected override Task ValidateAsync(ValidationContext<string> validationContext, PasswordConstraint constraint, ValidationBuilder builder, CancellationToken cancellationToken = new CancellationToken())
        {
            var password = validationContext.Subject;

            if (!HasNumberRegex.IsMatch(password))
                builder.AddError($"{validationContext.Path}", "Password has to contain number.");

            if (!HasUpperCaseRegex.IsMatch(password))
                builder.AddError($"{validationContext.Path}", "Password has to contain uppercase letter.");

            if (!HasMinimum8CharsRegex.IsMatch(password))
                builder.AddError($"{validationContext.Path}", "Password has be at least 8 characters long.");

            return Task.CompletedTask;
        }
    }
    public class PasswordConstraint : Constraint<PasswordConstraintValidator>
    {

    }
}