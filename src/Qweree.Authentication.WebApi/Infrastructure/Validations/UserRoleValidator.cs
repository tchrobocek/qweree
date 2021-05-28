using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Authentication.AdminSdk.Authorization.Roles;
using Qweree.Authentication.WebApi.Domain.Authorization.Roles;
using Qweree.Mongo.Exception;
using Qweree.Validator;
using UserRole = Qweree.Authentication.WebApi.Domain.Authorization.Roles.UserRole;

namespace Qweree.Authentication.WebApi.Infrastructure.Validations
{
    public class CreateUserRoleValidator : ObjectValidatorBase<CreateUserRoleInput>
    {
        private readonly IUserRoleRepository _userRoleRepository;

        public CreateUserRoleValidator(IUserRoleRepository userRoleRepository)
        {
            _userRoleRepository = userRoleRepository;
        }

        protected override async Task ValidateAsync(ValidationContext<CreateUserRoleInput> validationContext,
            ValidationBuilder builder, CancellationToken cancellationToken)
        {
            var input = validationContext.Subject;

            if (input == null)
            {
                return;
            }

            if (input.IsGroup)
            {
                var parentRoles = (await _userRoleRepository.FindParentRolesAsync(input.Id, cancellationToken))
                    .ToArray();

                if (parentRoles.Any())
                    builder.AddError(input.Key,
                        $@"Role cannot be group, because it already is item of another role(s). [{string.Join(", ", parentRoles.Select(r => r.Key))}]");

                var items = new List<UserRole>();
                foreach (var itemId in input.Items)
                {
                    if (itemId == input.Id)
                    {
                        builder.AddError(validationContext.Path, @"Role cannot reference itself.");
                        continue;
                    }

                    try
                    {
                        items.Add(await _userRoleRepository.GetAsync(itemId, cancellationToken));
                    }
                    catch (DocumentNotFoundException)
                    {
                        builder.AddError(input.Key, @$"Role ""{itemId}"" was not found.");
                    }
                }

                var parentsInItems = items.Where(i => i.IsGroup)
                    .ToArray();

                if (parentsInItems.Any())
                    builder.AddError(input.Key,
                        @$"Role ""{input.Key}"" cannot contain a group. [{string.Join(", ", parentsInItems.Select(r => r.Key).ToArray())}]");
            }
        }
    }

    public class ModifyUserRoleValidator : ObjectValidatorBase<ModifyUserRoleInput>
    {
        private readonly IUserRoleRepository _userRoleRepository;

        public ModifyUserRoleValidator(IUserRoleRepository userRoleRepository)
        {
            _userRoleRepository = userRoleRepository;
        }

        protected override async Task ValidateAsync(ValidationContext<ModifyUserRoleInput> validationContext,
            ValidationBuilder builder, CancellationToken cancellationToken)
        {
            var input = validationContext.Subject;

            if (input == null)
            {
                return;
            }

            if (input.IsGroup ?? false)
            {
                var parentRoles = (await _userRoleRepository.FindParentRolesAsync(input.Id, cancellationToken))
                    .ToArray();

                if (parentRoles.Any())
                    builder.AddError(validationContext.Path,
                        $@"Role cannot be group, because it already is item of another role(s). [{string.Join(", ", parentRoles.Select(r => r.Key))}]");


                if (input.Items != null)
                {
                    var items = new List<UserRole>();

                    foreach (var itemId in input.Items)
                    {
                        if (itemId == input.Id)
                        {
                            builder.AddError(validationContext.Path, @"Role cannot reference itself.");
                            continue;
                        }

                        try
                        {
                            items.Add(await _userRoleRepository.GetAsync(itemId, cancellationToken));
                        }
                        catch (DocumentNotFoundException)
                        {
                            builder.AddError(validationContext.Path, @$"Role ""{itemId}"" was not found.");
                        }
                    }

                    var parentsInItems = items.Where(i => i.IsGroup)
                        .ToArray();

                    if (parentsInItems.Any())
                        builder.AddError(validationContext.Path,
                            @$"Role cannot contain a group. [{string.Join(", ", parentsInItems.Select(r => r.Key).ToArray())}]");
                }
            }
        }
    }
}