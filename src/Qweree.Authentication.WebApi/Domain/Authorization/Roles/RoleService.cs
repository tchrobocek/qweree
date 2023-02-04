using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Qweree.AspNet.Application;
using Qweree.AspNet.Validations;
using Qweree.Mongo.Exception;
using Qweree.Utils;
using Qweree.Validator;

namespace Qweree.Authentication.WebApi.Domain.Authorization.Roles;

public class RoleService
{
    private readonly IRoleRepository _roleRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IValidator _validator;

    public RoleService(IRoleRepository roleRepository,
        IDateTimeProvider dateTimeProvider, IValidator validator)
    {
        _roleRepository = roleRepository;
        _dateTimeProvider = dateTimeProvider;
        _validator = validator;
    }

    public async Task<CollectionResponse<Role>> RolesFindAsync(CancellationToken cancellationToken = new())
    {
        var roles = await _roleRepository.FindAsync(cancellationToken);
        return Response.Ok(roles);
    }

    public async Task<Response<Role>> RoleCreateAsync(RoleCreateInput input,
        CancellationToken cancellationToken = new())
    {
        var validationResult = await _validator.ValidateAsync(input, cancellationToken);
        if (validationResult.HasFailed)
            return validationResult.ToErrorResponse<Role>();

        var id = input.Id;

        if (id == Guid.Empty)
            id = Guid.NewGuid();

        var role = new Role(id, input.Key, input.Label, input.Description, input.Items, input.IsGroup,
            _dateTimeProvider.UtcNow, _dateTimeProvider.UtcNow);

        try
        {
            await _roleRepository.InsertAsync(role, cancellationToken);
        }
        catch (InsertDocumentException e)
        {
            return Response.Fail<Role>(e.Message);
        }

        return Response.Ok(role);
    }

    public async Task<Response> RoleDeleteAsync(Guid id, CancellationToken cancellationToken = new())
    {
        await _roleRepository.DeleteOneAsync(id, cancellationToken);
        return Response.Ok();
    }

    public async Task<Response<Role>> RoleModifyAsync(RoleModifyInput input,
        CancellationToken cancellationToken = new())
    {
        var validationResult = await _validator.ValidateAsync(input, cancellationToken);
        if (validationResult.HasFailed)
            return validationResult.ToErrorResponse<Role>();

        Role role;

        try
        {
            role = await _roleRepository.GetAsync(input.Id, cancellationToken);
        }
        catch (DocumentNotFoundException e)
        {
            return Response.Fail<Role>(new Error(e.Message, 404));
        }

        var items = new List<Guid>();
        items.AddRange(input.IsGroup ?? role.IsGroup ? (input.Items ?? ImmutableArray<Guid>.Empty) : role.Items);
        role = new Role(role.Id, role.Key, input.Label ?? role.Label, input.Description ?? role.Description, items.ToImmutableArray(), input.IsGroup ?? role.IsGroup,
            role.CreatedAt, _dateTimeProvider.UtcNow);

        await _roleRepository.ReplaceAsync(role.Id.ToString(), role, cancellationToken);

        return Response.Ok(role);
    }
}