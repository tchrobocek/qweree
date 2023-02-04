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
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IValidator _validator;

    public RoleService(IUserRoleRepository userRoleRepository,
        IDateTimeProvider dateTimeProvider, IValidator validator)
    {
        _userRoleRepository = userRoleRepository;
        _dateTimeProvider = dateTimeProvider;
        _validator = validator;
    }

    public async Task<CollectionResponse<UserRole>> UserRolesFindAsync(CancellationToken cancellationToken = new())
    {
        var roles = await _userRoleRepository.FindAsync(cancellationToken);
        return Response.Ok(roles);
    }

    public async Task<Response<UserRole>> UserRoleCreateAsync(UserRoleCreateInput input,
        CancellationToken cancellationToken = new())
    {
        var validationResult = await _validator.ValidateAsync(input, cancellationToken);
        if (validationResult.HasFailed)
            return validationResult.ToErrorResponse<UserRole>();

        var id = input.Id;

        if (id == Guid.Empty)
            id = Guid.NewGuid();

        var role = new UserRole(id, input.Key, input.Label, input.Description, input.Items, input.IsGroup,
            _dateTimeProvider.UtcNow, _dateTimeProvider.UtcNow);

        try
        {
            await _userRoleRepository.InsertAsync(role, cancellationToken);
        }
        catch (InsertDocumentException e)
        {
            return Response.Fail<UserRole>(e.Message);
        }

        return Response.Ok(role);
    }

    public async Task<Response> UserRoleDeleteAsync(Guid id, CancellationToken cancellationToken = new())
    {
        await _userRoleRepository.DeleteOneAsync(id, cancellationToken);
        return Response.Ok();
    }

    public async Task<Response<UserRole>> UserRoleModifyAsync(UserRoleModifyInput input,
        CancellationToken cancellationToken = new())
    {
        var validationResult = await _validator.ValidateAsync(input, cancellationToken);
        if (validationResult.HasFailed)
            return validationResult.ToErrorResponse<UserRole>();

        UserRole role;

        try
        {
            role = await _userRoleRepository.GetAsync(input.Id, cancellationToken);
        }
        catch (DocumentNotFoundException e)
        {
            return Response.Fail<UserRole>(new Error(e.Message, 404));
        }

        var items = new List<Guid>();
        items.AddRange(input.IsGroup ?? role.IsGroup ? (input.Items ?? ImmutableArray<Guid>.Empty) : role.Items);
        role = new UserRole(role.Id, role.Key, input.Label ?? role.Label, input.Description ?? role.Description, items.ToImmutableArray(), input.IsGroup ?? role.IsGroup,
            role.CreatedAt, _dateTimeProvider.UtcNow);

        await _userRoleRepository.ReplaceAsync(role.Id.ToString(), role, cancellationToken);

        return Response.Ok(role);
    }
}