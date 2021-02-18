using System.Threading;
using System.Threading.Tasks;
using Qweree.AspNet.Application;
using Qweree.AspNet.Session;

namespace Qweree.Qwill.WebApi.Domain.Publishers
{
    public class ChannelService
    {
        private readonly IChannelRepository _channelRepository;
        private readonly ISessionStorage _sessionStorage;

        public ChannelService(IChannelRepository channelRepository, ISessionStorage sessionStorage)
        {
            _channelRepository = channelRepository;
            _sessionStorage = sessionStorage;
        }

        public async Task<CollectionResponse<Channel>> GetOwnChannelsAsync(CancellationToken cancellationToken = new())
        {
            var user = _sessionStorage.CurrentUser;
            var channels = await _channelRepository.FindUserAuthorAsync(user.Id, cancellationToken);
            return Response.Ok(channels);
        }
    }
}