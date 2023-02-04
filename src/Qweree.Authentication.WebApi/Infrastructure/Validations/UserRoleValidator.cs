using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Authentication.WebApi.Domain.Authorization.Roles;
using Qweree.Mongo.Exception;
using Qweree.Validator;
using Role = Qweree.Authentication.WebApi.Domain.Authorization.Roles.Role;

namespace Qweree.Authentication.WebApi.Infrastructure.Validations;

public class CreateRoleValidator : ObjectValidatorBase<RoleCreateInput>
{
    private readonly IRoleRepository _roleRepository;

    public CreateRoleValidator(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    protected override async Task ValidateAsync(ValidationContext<RoleCreateInput> validationContext,
        ValidationBuilder builder, CancellationToken cancellationToken)
    {
        var input = validationContext.Subject;

        if (input is null)
        {
            return;
        }

        if (input.IsGroup)
        {
            var parentRoles = (await _roleRepository.FindParentRolesAsync(input.Id, cancellationToken))
                .ToArray();

            if (parentRoles.Any())
                builder.AddError(input.Key,
                    $@"Role cannot be group, because it already is item of another role(s). [{string.Join(", ", parentRoles.Select(r => r.Key))}]");

            var items = new List<Role>();
            foreach (var itemId in input.Items)
            {
                if (itemId == input.Id)
                {
                    builder.AddError(validationContext.Path, @"Role cannot reference itself.");
                    continue;
                }

                try
                {
                    items.Add(await _roleRepository.GetAsync(itemId, cancellationToken));
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

public class ModifyRoleValidator : ObjectValidatorBase<RoleModifyInput>
{
    private readonly IRoleRepository _roleRepository;

    public ModifyRoleValidator(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    protected override async Task ValidateAsync(ValidationContext<RoleModifyInput> validationContext,
        ValidationBuilder builder, CancellationToken cancellationToken)
    {
        var input = validationContext.Subject;

        if (input is null)
        {
            return;
        }

        if (input.IsGroup ?? false)
        {
            var parentRoles = (await _roleRepository.FindParentRolesAsync(input.Id, cancellationToken))
                .ToArray();

            if (parentRoles.Any())
                builder.AddError(validationContext.Path,
                    $@"Role cannot be group, because it already is item of another role(s). [{string.Join(", ", parentRoles.Select(r => r.Key))}]");


            if (input.Items is not null)
            {
                var items = new List<Role>();

                foreach (var itemId in input.Items)
                {
                    if (itemId == input.Id)
                    {
                        builder.AddError(validationContext.Path, @"Role cannot reference itself.");
                        continue;
                    }

                    try
                    {
                        items.Add(await _roleRepository.GetAsync(itemId, cancellationToken));
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