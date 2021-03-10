using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Qweree.AspNet.Application;
using Qweree.AspNet.Session;
using Qweree.Authentication.AdminSdk.Identity.Users;
using Qweree.Authentication.WebApi.Domain.Security;
using Qweree.Mongo;
using Qweree.Mongo.Exception;
using Qweree.Utils;
using Qweree.Validator;
using SdkUser = Qweree.Authentication.AdminSdk.Identity.Users.User;

namespace Qweree.Authentication.WebApi.Domain.Identity
{
    public class UserService
    {
        private readonly IDateTimeProvider _datetimeProvider;
        private readonly IUserRepository _userRepository;
        private readonly IValidator _validator;
        private readonly ISessionStorage _sessionStorage;
        private readonly IPasswordEncoder _passwordEncoder;
        private readonly SdkMapperService _sdkMapperService;

        public UserService(IDateTimeProvider datetimeProvider, IUserRepository userRepository, IValidator validator,
            ISessionStorage sessionStorage, IPasswordEncoder passwordEncoder, SdkMapperService sdkMapperService)
        {
            _datetimeProvider = datetimeProvider;
            _userRepository = userRepository;
            _validator = validator;
            _sessionStorage = sessionStorage;
            _passwordEncoder = passwordEncoder;
            _sdkMapperService = sdkMapperService;
        }

        public async Task<Response<SdkUser>> CreateUserAsync(UserCreateInput userCreateInput,
            CancellationToken cancellationToken = new())
        {
            var validationResult = await _validator.ValidateAsync(userCreateInput, cancellationToken);
            if (validationResult.HasFailed)
                return Response.Fail<SdkUser>(validationResult.Errors.Select(e => $"{e.Path} - {e.Message}."));

            var password = _passwordEncoder.EncodePassword(userCreateInput.Password);
            var user = new User(Guid.NewGuid(), userCreateInput.Username, userCreateInput.FullName,
                userCreateInput.ContactEmail,
                password, userCreateInput.Roles, _datetimeProvider.UtcNow, _datetimeProvider.UtcNow);

            try
            {
                await _userRepository.InsertAsync(user, cancellationToken);
            }
            catch (InsertDocumentException)
            {
                return Response.Fail<SdkUser>("User is duplicate.");
            }

            return Response.Ok(_sdkMapperService.MapUser(user));
        }

        public async Task<Response<SdkUser>> GetUserAsync(Guid userId, CancellationToken cancellationToken = new())
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

            return Response.Ok(_sdkMapperService.MapUser(user));
        }

        public async Task<PaginationResponse<SdkUser>> FindUsersAsync(FindUsersInput input,
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

            return Response.Ok(pagination.Documents.Select(_sdkMapperService.MapUser), pagination.TotalCount);
        }

        public async Task<Response> DeleteAsync(Guid id, CancellationToken cancellationToken = new())
        {
            if (_sessionStorage.CurrentUser.Id == id)
            {
                return Response.Fail("Cannot delete self.");
            }

            try
            {
                await _userRepository.DeleteAsync(id, cancellationToken);
            }
            catch (Exception)
            {
                return Response.Fail($@"Cannot delete user ""{id}"".");
            }

            return Response.Ok();
        }
    }
}