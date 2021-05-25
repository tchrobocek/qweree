using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Qweree.AspNet.Application;
using Qweree.Authentication.AdminSdk.Authorization.Roles;
using Qweree.Mongo.Exception;
using Qweree.Utils;
using SdkUserRole = Qweree.Authentication.AdminSdk.Authorization.Roles.UserRole;
using SdkClientRole = Qweree.Authentication.AdminSdk.Authorization.Roles.ClientRole;

namespace Qweree.Authentication.WebApi.Domain.Authorization.Roles
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

        public async Task<CollectionResponse<SdkUserRole>> FindUserRolesAsync(CancellationToken cancellationToken = new())
        {
            var roles = await _userRoleRepository.FindAsync(cancellationToken);
            var sdkRoles = new List<SdkUserRole>();

            foreach (var role in roles)
                sdkRoles.Add(await _sdkMapperService.MapUserRoleAsync(role, cancellationToken));

            return Response.Ok((IEnumerable<SdkUserRole>)sdkRoles);
        }

        public async Task<CollectionResponse<SdkClientRole>> FindClientRolesAsync(
            CancellationToken cancellationToken = new())
        {
            var roles = await _clientRoleRepository.FindAsync(cancellationToken);
            var sdkRoles = new List<SdkClientRole>();

            foreach (var role in roles)
                sdkRoles.Add(await _sdkMapperService.MapClientRoleAsync(role, cancellationToken));

            return Response.Ok((IEnumerable<SdkClientRole>)sdkRoles);
        }

        public async Task<Response<SdkUserRole>> CreateUserRoleAsync(CreateUserRoleInput input,
            CancellationToken cancellationToken = new())
        {
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
                return Response.Fail<SdkUserRole>(e.Message);
            }

            return Response.Ok(await _sdkMapperService.MapUserRoleAsync(role, cancellationToken));
        }

        public async Task<Response<SdkClientRole>> CreateClientRoleAsync(CreateClientRoleInput input,
            CancellationToken cancellationToken = new())
        {
            var id = input.Id;

            if (id == Guid.Empty)
                id = Guid.NewGuid();

            var role = new ClientRole(id, input.Key, input.Label, input.Description, input.Items, input.IsGroup,
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

        public async Task<Response> DeleteUserRoleAsync(Guid id, CancellationToken cancellationToken = new())
        {
            await _userRoleRepository.DeleteAsync(id, cancellationToken);
            return Response.Ok();
        }

        public async Task<Response> DeleteClientRoleAsync(Guid id, CancellationToken cancellationToken = new())
        {
            await _clientRoleRepository.DeleteAsync(id, cancellationToken);
            return Response.Ok();
        }

        public async Task<Response<SdkUserRole>> ModifyUserRoleAsync(Guid id, ModifyUserRoleInput input,
            CancellationToken cancellationToken = new())
        {
            UserRole role;

            try
            {
                role = await _userRoleRepository.GetAsync(id, cancellationToken);
            }
            catch (DocumentNotFoundException e)
            {
                return Response.Fail<SdkUserRole>(new Error(e.Message, 404));
            }

            var items = new List<Guid>();
            items.AddRange(input.IsGroup ?? role.IsGroup ? (input.Items ?? ImmutableArray<Guid>.Empty) : role.Items);
            role = new UserRole(role.Id, role.Key, input.Label ?? role.Label, input.Description ?? role.Description, items.ToImmutableArray(), input.IsGroup ?? role.IsGroup,
                role.CreatedAt, _dateTimeProvider.UtcNow);

            await _userRoleRepository.ReplaceAsync(role.Id.ToString(), role, cancellationToken);

            return Response.Ok(await _sdkMapperService.MapUserRoleAsync(role, cancellationToken));
        }

        public async Task<Response<SdkClientRole>> ModifyClientRoleAsync(Guid id, ModifyClientRoleInput input,
            CancellationToken cancellationToken = new())
        {
            ClientRole role;

            try
            {
                role = await _clientRoleRepository.GetAsync(id, cancellationToken);
            }
            catch (DocumentNotFoundException e)
            {
                return Response.Fail<SdkClientRole>(e.Message);
            }

            var items = new List<Guid>();
            items.AddRange(input.IsGroup ?? role.IsGroup ? (input.Items ?? ImmutableArray<Guid>.Empty) : role.Items);
            role = new ClientRole(role.Id, role.Key, input.Label ?? role.Label, input.Description ?? role.Description, items.ToImmutableArray(), input.IsGroup ?? role.IsGroup,
                role.CreatedAt, _dateTimeProvider.UtcNow);

            await _clientRoleRepository.ReplaceAsync(role.Id.ToString(), role, cancellationToken);

            return Response.Ok(await _sdkMapperService.MapClientRoleAsync(role, cancellationToken));
        }
    }
}