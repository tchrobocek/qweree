using Qweree.Authentication.AdminSdk.Authorization.Roles;
using Qweree.Authentication.AdminSdk.Identity.Users;
using Qweree.Authentication.WebApi.Infrastructure.Authorization.Roles;
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
                    .AddConstraint(new NotEmptyConstraint("Username cannot be empty."))
                    .AddConstraint(new MinLengthConstraint(3, "Username has to have 3 or more characters."))
                    .AddConstraint(new MaxLengthConstraint(255, "Username has to have up to 255 characters."));
                c.AddProperty(p => p.ContactEmail)
                    .AddConstraint(new NotEmptyConstraint("Username cannot be empty."))
                    .AddConstraint(new EmailConstraint("Email cannot be empty.", "Email is not valid email address."));
                c.AddProperty(p => p.Password)
                    .AddConstraint(new NotEmptyConstraint("Password cannot be empty."))
                    .AddConstraint(new PasswordConstraint());
            });

            builder.AddModel<UserRoleCreateInput>(c =>
            {
                c.AddProperty(p => p.Key)
                    .AddConstraint(new UniqueConstraint(typeof(UserRoleRepository)))
                    .AddConstraint(new NotEmptyConstraint("Key cannot be empty."))
                    .AddConstraint(new MinLengthConstraint(3, "Key has to have 3 or more characters."))
                    .AddConstraint(new MaxLengthConstraint(255, "Key has to have up to 255 characters."));
                c.AddProperty(p => p.Label)
                    .AddConstraint(new NotEmptyConstraint("Label cannot be empty."));
            });

            builder.AddModel<ClientRoleCreateInput>(c =>
            {
                c.AddProperty(p => p.Key)
                    .AddConstraint(new UniqueConstraint(typeof(ClientRoleRepository)))
                    .AddConstraint(new NotEmptyConstraint("Key cannot be empty."))
                    .AddConstraint(new MinLengthConstraint(3, "Key has to have 3 or more characters."))
                    .AddConstraint(new MaxLengthConstraint(255, "Key has to have up to 255 characters."));
                c.AddProperty(p => p.Label)
                    .AddConstraint(new NotEmptyConstraint("Label cannot be empty."));
            });

            builder.AddModel<UserRoleModifyInput>(c =>
            {
                c.AddProperty(p => p.Label)
                    .AddConstraint(new NotEmptyConstraint("Label cannot be empty."));
            });

            builder.AddModel<ClientRoleModifyInput>(c =>
            {
                c.AddProperty(p => p.Label)
                    .AddConstraint(new NotEmptyConstraint("Label cannot be empty."));
            });
        }
    }
}