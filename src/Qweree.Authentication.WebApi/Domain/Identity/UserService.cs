using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Qweree.AspNet.Application;
using Qweree.Authentication.Sdk.Session;
using Qweree.Authentication.WebApi.Domain.Authorization;
using Qweree.Authentication.WebApi.Domain.Authorization.Roles;
using Qweree.Mongo;
using Qweree.Mongo.Exception;

namespace Qweree.Authentication.WebApi.Domain.Identity;

public class UserService
{
    private readonly IUserRepository _userRepository;
    private readonly ISessionStorage _sessionStorage;
    private readonly AuthorizationService _authorizationService;

    public UserService(IUserRepository userRepository, ISessionStorage sessionStorage, AuthorizationService authorizationService)
    {
        _userRepository = userRepository;
        _sessionStorage = sessionStorage;
        _authorizationService = authorizationService;
    }

    public async Task<Response<User>> UserGetAsync(Guid userId, CancellationToken cancellationToken = new())
    {
        User user;

        try
        {
            user = await _userRepository.GetAsync(userId, cancellationToken);
        }
        catch (DocumentNotFoundException)
        {
            return Response.Fail<User>(new Error($@"User ""{userId}"" was not found.",
                StatusCodes.Status404NotFound));
        }

        return Response.Ok(user);
    }

    public async Task<PaginationResponse<User>> UsersPaginateAsync(UserFindInput input,
        CancellationToken cancellationToken = new())
    {
        Pagination<User> pagination;

        try
        {
            pagination = await _userRepository.PaginateAsync(input.Skip, input.Take, input.Sort, cancellationToken);
        }
        catch (Exception e)
        {
            return Response.FailPagination<User>(e.Message);
        }

        return Response.Ok(pagination.Documents, pagination.TotalCount);
    }

    public async Task<Response> UserDeleteAsync(Guid id, CancellationToken cancellationToken = new())
    {
        if (_sessionStorage.CurrentUser?.Id == id)
        {
            return Response.Fail("Cannot delete self.");
        }

        try
        {
            await _userRepository.DeleteOneAsync(id, cancellationToken);
        }
        catch (Exception)
        {
            return Response.Fail($@"Cannot delete user ""{id}"".");
        }

        return Response.Ok();
    }

    public async Task<CollectionResponse<UserRole>> UserGetEffectiveRolesAsync(Guid id, CancellationToken cancellationToken = new())
    {
        User user;

        try
        {
            user = await _userRepository.GetAsync(id, cancellationToken);
        }
        catch (DocumentNotFoundException)
        {
            return Response.FailCollection<UserRole>(new Error($@"User ""{id}"" was not found.",
                StatusCodes.Status404NotFound));
        }

        var effectiveRoles = new List<UserRole>();

        await foreach (var effectiveRole in _authorizationService.GetEffectiveUserRoles(user, cancellationToken)
                           .WithCancellation(cancellationToken))
        {
            effectiveRoles.Add(effectiveRole);
        }

        return Response.Ok((IEnumerable<UserRole>)effectiveRoles);
    }
}