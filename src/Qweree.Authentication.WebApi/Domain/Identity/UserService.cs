using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Qweree.AspNet.Application;
using Qweree.Authentication.AdminSdk.Authorization.Roles;
using Qweree.Authentication.Sdk.Session;
using Qweree.Authentication.WebApi.Domain.Authorization;
using Qweree.Mongo;
using Qweree.Mongo.Exception;
using SdkUser = Qweree.Authentication.AdminSdk.Identity.Users.User;

namespace Qweree.Authentication.WebApi.Domain.Identity;

public class UserService
{
    private readonly IUserRepository _userRepository;
    private readonly ISessionStorage _sessionStorage;
    private readonly SdkMapperService _sdkMapperService;
    private readonly AuthorizationService _authorizationService;

    public UserService(IUserRepository userRepository, ISessionStorage sessionStorage, SdkMapperService sdkMapperService, AuthorizationService authorizationService)
    {
        _userRepository = userRepository;
        _sessionStorage = sessionStorage;
        _sdkMapperService = sdkMapperService;
        _authorizationService = authorizationService;
    }

    public async Task<Response<SdkUser>> UserGetAsync(Guid userId, CancellationToken cancellationToken = new())
    {
        User user;

        try
        {
            user = await _userRepository.GetAsync(userId, cancellationToken);
        }
        catch (DocumentNotFoundException)
        {
            return Response.Fail<SdkUser>(new Error($@"User ""{userId}"" was not found.",
                StatusCodes.Status404NotFound));
        }

        return Response.Ok(await _sdkMapperService.UserMapAsync(user, cancellationToken));
    }

    public async Task<PaginationResponse<SdkUser>> UsersPaginateAsync(UserFindInput input,
        CancellationToken cancellationToken = new())
    {
        Pagination<User> pagination;

        try
        {
            pagination = await _userRepository.PaginateAsync(input.Skip, input.Take, input.Sort, cancellationToken);
        }
        catch (Exception e)
        {
            return Response.FailPagination<SdkUser>(e.Message);
        }

        var users = new List<SdkUser>();
        foreach (var document in pagination.Documents)
        {
            users.Add(await _sdkMapperService.UserMapAsync(document, cancellationToken));
        }

        return Response.Ok(users, pagination.TotalCount);
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

    public async Task<CollectionResponse<Role>> UserGetEffectiveRolesAsync(Guid id, CancellationToken cancellationToken = new())
    {
        User user;

        try
        {
            user = await _userRepository.GetAsync(id, cancellationToken);
        }
        catch (DocumentNotFoundException)
        {
            return Response.FailCollection<Role>(new Error($@"User ""{id}"" was not found.",
                StatusCodes.Status404NotFound));
        }

        var effectiveRoles = new List<Role>();

        await foreach (var effectiveRole in _authorizationService.GetEffectiveUserRoles(user, cancellationToken)
                           .WithCancellation(cancellationToken))
        {
            effectiveRoles.Add(effectiveRole);
        }

        return Response.Ok((IEnumerable<Role>)effectiveRoles);
    }
}