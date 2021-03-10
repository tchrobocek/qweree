using System.Threading;
using System.Threading.Tasks;
using Qweree.Authentication.AdminSdk.Identity.Clients;
using Qweree.Authentication.WebApi.Domain.Identity;
using Client = Qweree.Authentication.WebApi.Domain.Identity.Client;
using SdkClient = Qweree.Authentication.AdminSdk.Identity.Clients.Client;
using SdkUser = Qweree.Authentication.AdminSdk.Identity.Users.User;

namespace Qweree.Authentication.WebApi.Domain
{
    public class SdkMapperService
    {
        private readonly IUserRepository _userRepository;

        public SdkMapperService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public SdkUser MapUser(User user)
        {
            return new(user.Id, user.Username, user.FullName, user.ContactEmail, user.Roles, user.CreatedAt, user.ModifiedAt);
        }

        public async Task<SdkClient> MapClientAsync(Client client, CancellationToken cancellationToken = new())
        {
            var owner = await _userRepository.GetAsync(client.OwnerId, cancellationToken);
            return new(client.Id, client.ClientId, client.ApplicationName, client.Origin, MapUser(owner), client.CreatedAt, client.ModifiedAt);
        }

        public async Task<CreatedClient> MapToCreatedClientAsync(Client client, CancellationToken cancellationToken = new())
        {
            var owner = await _userRepository.GetAsync(client.OwnerId, cancellationToken);
            return new(client.Id, client.ClientId, client.ApplicationName, client.Origin, MapUser(owner), client.CreatedAt, client.ModifiedAt);
        }
    }
}