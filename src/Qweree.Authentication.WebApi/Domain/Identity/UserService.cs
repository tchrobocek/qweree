using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Qweree.AspNet.Application;
using Qweree.Authentication.Sdk.Identity;
using Qweree.Mongo;
using Qweree.Mongo.Exception;
using Qweree.Utils;
using Qweree.Validator;

namespace Qweree.Authentication.WebApi.Domain.Identity
{
    public class UserService
    {
        private readonly IDateTimeProvider _datetimeProvider;
        private readonly IUserRepository _userRepository;
        private readonly IValidator _validator;

        public UserService(IDateTimeProvider datetimeProvider, IUserRepository userRepository, IValidator validator)
        {
            _datetimeProvider = datetimeProvider;
            _userRepository = userRepository;
            _validator = validator;
        }

        public async Task<Response<User>> CreateUserAsync(UserCreateInput userCreateInput, CancellationToken cancellationToken = new CancellationToken())
        {
            var validationResult = await _validator.ValidateAsync(userCreateInput, cancellationToken);
            if (validationResult.HasFailed)
                return Response.Fail<User>(validationResult.Errors.Select(e => $"{e.Path} - {e.Message}."));

            var password = EncryptPassword(userCreateInput.Password);
            var user = new User(Guid.NewGuid(), userCreateInput.Username, userCreateInput.FullName, userCreateInput.ContactEmail,
                password, userCreateInput.Roles, _datetimeProvider.UtcNow, _datetimeProvider.UtcNow);

            try
            {
                await _userRepository.InsertAsync(user, cancellationToken);
            }
            catch (InsertDocumentException)
            {
                return Response.Fail<User>("User is duplicate.s");
            }

            return Response.Ok(user);
        }

        public async Task<Response<User>> FindUserAsync(Guid userId, CancellationToken cancellationToken = new CancellationToken())
        {
            User user;

            try
            {
                user = await _userRepository.GetAsync(userId, cancellationToken);
            }
            catch (DocumentNotFoundException)
            {
                return Response.Fail<User>(new Error($@"User ""{userId}"" was not found."));
            }

            return Response.Ok(user);
        }

        public async Task<PaginationResponse<User>> FindUsersAsync(FindUsersInput input, CancellationToken cancellationToken)
        {
            Pagination<User> pagination;

            try
            {
                pagination = await _userRepository.PaginateAsync(input.Take, input.Skip, input.Sort, cancellationToken);
            }
            catch (Exception e)
            {
                return Response.FailPagination<User>(e.Message);
            }

            return Response.Ok(pagination.Documents, pagination.TotalCount);
        }

        private string EncryptPassword(string password)
        {
            if (password == string.Empty)
            {
                return string.Empty;
            }

            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}