using System.Threading;
using System.Threading.Tasks;
using Qweree.AspNet.Application;
using Qweree.Authentication.AdminSdk.Authorization;
using Qweree.Mongo.Exception;
using Qweree.Utils;
using SdkUserRole = Qweree.Authentication.AdminSdk.Authorization.UserRole;
using SdkClientRole = Qweree.Authentication.AdminSdk.Authorization.ClientRole;

namespace Qweree.Authentication.WebApi.Domain.Authorization
{
    public class RoleService
    {
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IClientRoleRepository _clientRoleRepository;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly SdkMapperService _sdkMapperService;

        public RoleService(IUserRoleRepository userRoleRepository, IClientRoleRepository clientRoleRepository,
            IDateTimeProvider dateTimeProvider, SdkMapperService sdkMapperService)
        {
            _userRoleRepository = userRoleRepository;
            _clientRoleRepository = clientRoleRepository;
            _dateTimeProvider = dateTimeProvider;
            _sdkMapperService = sdkMapperService;
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

        public async Task<Response<SdkUserRole>> CreateUserRoleAsync(CreateUserRoleInput input,
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
                return Response.Fail<SdkUserRole>(e.Message);
            }

            return Response.Ok(await _sdkMapperService.MapUserRoleAsync(role, cancellationToken));
        }

        public async Task<Response<SdkClientRole>> CreateClientRoleAsync(CreateClientRoleInput input,
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
                return Response.Fail<SdkClientRole>(e.Message);
            }

            return Response.Ok(await _sdkMapperService.MapClientRoleAsync(role, cancellationToken));
        }
    }
}