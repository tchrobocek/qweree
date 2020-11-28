using Qweree.Authentication.WebApi.Application.Identity;
using Qweree.Authentication.WebApi.Infrastructure.Identity;
using Qweree.Authentication.WebApi.Infrastructure.Validations;
using Qweree.Validator.Constraints;
using Qweree.Validator.ModelValidation.Static;

namespace Qweree.Authentication.WebApi.Infrastructure
{
    public class ValidationMap
    {
        public static void ConfigureValidator(ValidatorSettingsBuilder builder)
        {
            builder.AddModel<UserCreateInput>(c =>
            {
                c.AddProperty(p => p.Username)
                    .AddConstraint(new UniqueConstraint(typeof(UserRepository)))
                    .AddConstraint(new MinLengthConstraint(3, "Username has to have 3 or more characters."))
                    .AddConstraint(new MaxLengthConstraint(255, "Username has to have up to 255 characters."));
                c.AddProperty(p => p.ContactEmail)
                    .AddConstraint(new EmailConstraint("Email cannot be empty.", "Email is not valid email address."));
                c.AddProperty(p => p.Password)
                    .AddConstraint(new PasswordConstraint());
            });
        }
    }
}