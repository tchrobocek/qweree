using System.Threading;
using System.Threading.Tasks;
using Qweree.AspNet.Application;
using Qweree.Authentication.AdminSdk.Authorization;
using Qweree.Mongo.Exception;
using Qweree.Utils;

namespace Qweree.Authentication.WebApi.Domain.Authorization
{
    public class RoleService
    {
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IClientRoleRepository _clientRoleRepository;
        private readonly IDateTimeProvider _dateTimeProvider;

        public RoleService(IUserRoleRepository userRoleRepository, IClientRoleRepository clientRoleRepository,
            IDateTimeProvider dateTimeProvider)
        {
            _userRoleRepository = userRoleRepository;
            _clientRoleRepository = clientRoleRepository;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<CollectionResponse<UserRole>> FindUserRolesAsync(CancellationToken cancellationToken = new())
        {
            return Response.Ok(await _userRoleRepository.FindAsync(cancellationToken));
        }

        public async Task<CollectionResponse<ClientRole>> FindClientRolesAsync(
            CancellationToken cancellationToken = new())
        {
            return Response.Ok(await _clientRoleRepository.FindAsync(cancellationToken));
        }

        public async Task<Response<UserRole>> CreateUserRoleAsync(CreateUserRoleInput input,
            CancellationToken cancellationToken = new())
        {
            var role = new UserRole(input.Id, input.Key, input.Label, input.Description, input.Items, input.IsGroup,
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

        public async Task<Response<ClientRole>> CreateClientRoleAsync(CreateClientRoleInput input,
            CancellationToken cancellationToken = new())
        {
            var role = new ClientRole(input.Id, input.Key, input.Label, input.Description, input.Items, input.IsGroup,
                _dateTimeProvider.UtcNow, _dateTimeProvider.UtcNow);

            try
            {
                await _clientRoleRepository.InsertAsync(role, cancellationToken);
            }
            catch (InsertDocumentException e)
            {
                return Response.Fail<ClientRole>(e.Message);
            }

            return Response.Ok(role);
        }
    }
}