using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Authentication.AdminSdk.Authorization.Roles;
using Qweree.Authentication.WebApi.Domain.Authorization.Roles;
using Qweree.Mongo.Exception;
using Qweree.Validator;
using ClientRole = Qweree.Authentication.WebApi.Domain.Authorization.Roles.ClientRole;

namespace Qweree.Authentication.WebApi.Infrastructure.Validations
{
    public class CreateClientRoleValidator : ObjectValidatorBase<CreateClientRoleInput>
    {
        private readonly IClientRoleRepository _clientRoleRepository;

        public CreateClientRoleValidator(IClientRoleRepository clientRoleRepository)
        {
            _clientRoleRepository = clientRoleRepository;
        }

        protected override async Task ValidateAsync(ValidationContext<CreateClientRoleInput> validationContext,
            ValidationBuilder builder, CancellationToken cancellationToken)
        {
            var input = validationContext.Subject;

            if (input == null)
            {
                return;
            }

            if (input.IsGroup)
            {
                var parentRoles = (await _clientRoleRepository.FindParentRolesAsync(input.Id, cancellationToken))
                    .ToArray();

                if (parentRoles.Any())
                    builder.AddError(input.Key,
                        $@"Role cannot be group, because it already is item of another role(s). [{string.Join(", ", parentRoles.Select(r => r.Key))}]");

                var items = new List<ClientRole>();
                foreach (var itemId in input.Items)
                {
                    try
                    {
                        items.Add(await _clientRoleRepository.GetAsync(itemId, cancellationToken));
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

    public class ModifyClientRoleValidator : ObjectValidatorBase<ModifyClientRoleInput>
    {
        private readonly IClientRoleRepository _clientRoleRepository;

        public ModifyClientRoleValidator(IClientRoleRepository clientRoleRepository)
        {
            _clientRoleRepository = clientRoleRepository;
        }

        protected override async Task ValidateAsync(ValidationContext<ModifyClientRoleInput> validationContext,
            ValidationBuilder builder, CancellationToken cancellationToken)
        {
            var input = validationContext.Subject;

            if (input == null)
            {
                return;
            }

            if (input.IsGroup ?? false)
            {
                var parentRoles = (await _clientRoleRepository.FindParentRolesAsync(input.Id, cancellationToken))
                    .ToArray();

                if (parentRoles.Any())
                    builder.AddError(validationContext.Path,
                        $@"Role cannot be group, because it already is item of another role(s). [{string.Join(", ", parentRoles.Select(r => r.Key))}]");


                if (input.Items != null)
                {
                    var items = new List<ClientRole>();

                    foreach (var itemId in input.Items)
                    {
                        try
                        {
                            items.Add(await _clientRoleRepository.GetAsync(itemId, cancellationToken));
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