using System;
using System.Net.Mail;
using System.Threading.Tasks;
using Qweree.Validator.ModelValidation;
using Qweree.Validator.ModelValidation.Attributes;

namespace Qweree.Validator.Constraints
{
    public class EmailConstraintValidator : ConstraintValidatorBase<string, EmailConstraint>
    {
        protected override Task ValidateAsync(ValidationContext<string> context, EmailConstraint constraint,
            ValidationBuilder builder)
        {
            try
            {
                // ReSharper disable once ObjectCreationAsStatement
                new MailAddress(context.Subject);
            }
            catch (FormatException)
            {
                builder.AddError(context.Path, constraint.EmailNotValidMessage);
            }
            catch (ArgumentException)
            {
                builder.AddError(context.Path, constraint.EmailEmptyMessage);
            }

            return Task.CompletedTask;
        }
    }

    public class EmailConstraint : Constraint<EmailConstraintValidator>
    {
        public EmailConstraint(string emailEmptyMessage, string emailNotValidMessage)
        {
            EmailEmptyMessage = emailEmptyMessage;
            EmailNotValidMessage = emailNotValidMessage;
        }

        public string EmailEmptyMessage { get; }
        public string EmailNotValidMessage { get; }
    }

    public class EmailConstraintAttribute : ConstraintAttribute
    {
        public EmailConstraintAttribute()
        {
            EmailEmptyMessage = "Email Cannot be empty.";
            EmailNotValidMessage = "Email is not valid email address.";
        }

        public string EmailEmptyMessage { get; set; }
        public string EmailNotValidMessage { get; set; }

        public override IConstraint CreateConstraint()
        {
            return new EmailConstraint(EmailEmptyMessage, EmailNotValidMessage);
        }
    }
}