using Qweree.Authentication.AdminSdk.Authorization.Roles;
using Qweree.Authentication.AdminSdk.Identity.Clients;
using Qweree.Authentication.AdminSdk.Identity.Users;
using Qweree.Authentication.Sdk.Account;
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
                c.AddProperty(p => p.Roles)
                    .AddConstraint(new ExistsConstraint(typeof(UserRoleRepository)));
            });

            builder.AddModel<ClientCreateInput>(c =>
            {
                c.AddProperty(p => p.ClientId)
                    .AddConstraint(new UniqueConstraint(typeof(ClientRepository)))
                    .AddConstraint(new NotEmptyConstraint("Client id cannot be empty."))
                    .AddConstraint(new MinLengthConstraint(3, "Client id has to have 3 or more characters."))
                    .AddConstraint(new MaxLengthConstraint(255, "Client id has to have up to 255 characters."));
                c.AddProperty(p => p.Origin)
                    .AddConstraint(new MaxLengthConstraint(255, "Origin has to have up to 255 characters."));
                c.AddProperty(p => p.ApplicationName)
                    .AddConstraint(new NotEmptyConstraint("Application name cannot be empty."))
                    .AddConstraint(new MinLengthConstraint(3, "Application name has to have 3 or more characters."))
                    .AddConstraint(new MaxLengthConstraint(255, "Application name has to have up to 255 characters."));
                c.AddProperty(p => p.Roles)
                    .AddConstraint(new ExistsConstraint(typeof(ClientRoleRepository)));
                c.AddProperty(p => p.OwnerId)
                    .AddConstraint(new ExistsConstraint(typeof(UserRepository)));
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
                c.AddProperty(p => p.Items)
                    .AddConstraint(new ExistsConstraint(typeof(UserRoleRepository)));
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
                c.AddProperty(p => p.Items)
                    .AddConstraint(new ExistsConstraint(typeof(ClientRoleRepository)));
            });

            builder.AddModel<UserRoleModifyInput>(c =>
            {
                c.AddProperty(p => p.Label)
                    .AddConstraint(new NotEmptyConstraint("Label cannot be empty."));
                c.AddProperty(p => p.Items)
                    .AddConstraint(new ExistsConstraint(typeof(UserRoleRepository)));
            });

            builder.AddModel<ClientRoleModifyInput>(c =>
            {
                c.AddProperty(p => p.Label)
                    .AddConstraint(new NotEmptyConstraint("Label cannot be empty."));
                c.AddProperty(p => p.Items)
                    .AddConstraint(new ExistsConstraint(typeof(ClientRoleRepository)));
            });

            builder.AddModel<ChangeMyPasswordInput>(c =>
            {
                c.AddProperty(p => p.NewPassword)
                    .AddConstraint(new NotEmptyConstraint("Password cannot be empty."))
                    .AddConstraint(new PasswordConstraint());
            });
        }
    }
}