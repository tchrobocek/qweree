using System;
using System.Threading;
using System.Threading.Tasks;
using Qweree.AspNet.Application;
using Qweree.Authentication.AdminSdk.Identity.Users.UserRegister;
using Qweree.Authentication.WebApi.Infrastructure.Validations;
using Qweree.Mongo;
using Qweree.Mongo.Exception;
using Qweree.Validator;

namespace Qweree.Authentication.WebApi.Infrastructure.Identity.UserRegister
{
    public class UserInvitationService
    {
        private readonly IValidator _validator;
        private readonly UserInvitationRepository _userInvitationRepository;

        public UserInvitationService(IValidator validator, UserInvitationRepository userInvitationRepository)
        {
            _validator = validator;
            _userInvitationRepository = userInvitationRepository;
        }

        public async Task<Response<UserInvitation>> UserInvitationGetAsync(Guid id, CancellationToken cancellationToken = new())
        {
            UserInvitation invitation;
            try
            {
                invitation = await _userInvitationRepository.GetAsync(id, cancellationToken);
            }
            catch (DocumentNotFoundException)
            {
                return Response.Fail<UserInvitation>("User invitation was not found.");
            }
            return Response.Ok(invitation);
        }

        public async Task<Response<UserInvitation>> UserInvitationCreateAsync(UserInvitationInput input, CancellationToken cancellationToken = new())
        {
            var invitation = new UserInvitation(Guid.NewGuid(), input.Username, input.FullName, input.ContactEmail,
                input.Roles);

            var validationResult = await _validator.ValidateAsync(input, cancellationToken);
            if (validationResult.HasFailed)
                return validationResult.ToErrorResponse<UserInvitation>();

            try
            {
                await _userInvitationRepository.InsertAsync(invitation, cancellationToken);
            }
            catch (InsertDocumentException)
            {
                return Response.Fail<UserInvitation>("User invitation is a duplicate.");
            }

            return Response.Ok(invitation);
        }

        public async Task<PaginationResponse<UserInvitation>> UserInvitationsPaginateAsync(UserInvitationsFindInput input,
            CancellationToken cancellationToken = new())
        {
            Pagination<UserInvitation> pagination;

            try
            {
                pagination = await _userInvitationRepository.PaginateAsync(input.Skip, input.Take, input.Sort, cancellationToken);
            }
            catch (Exception e)
            {
                return Response.FailPagination<UserInvitation>(e.Message);
            }


            return Response.Ok(pagination.Documents, pagination.TotalCount);
        }

        public async Task<Response> UserInvitationDeleteAsync(Guid id, CancellationToken cancellationToken = new())
        {
            await _userInvitationRepository.DeleteAsync(id, cancellationToken);
            return Response.Ok();
        }
    }
}