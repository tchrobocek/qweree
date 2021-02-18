using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Qweree.AspNet.Application;
using Qweree.AspNet.Session;
using Qweree.Utils;
using Qweree.Validator;

namespace Qweree.Qwill.WebApi.Domain.Publishers
{
    public class ChannelService
    {
        private readonly IChannelRepository _channelRepository;
        private readonly ISessionStorage _sessionStorage;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IValidator _validator;

        public ChannelService(IChannelRepository channelRepository, ISessionStorage sessionStorage,
            IDateTimeProvider dateTimeProvider, IValidator validator)
        {
            _channelRepository = channelRepository;
            _sessionStorage = sessionStorage;
            _dateTimeProvider = dateTimeProvider;
            _validator = validator;
        }

        public async Task<CollectionResponse<Channel>> GetOwnChannelsAsync(CancellationToken cancellationToken = new())
        {
            var user = _sessionStorage.CurrentUser;
            var channels = await _channelRepository.FindUserAuthorAsync(user.Id, cancellationToken);
            return Response.Ok(channels);
        }

        public async Task<Response<Channel>> CreateAsync(ChannelCreateInput input,
            CancellationToken cancellationToken = new())
        {
            var user = _sessionStorage.CurrentUser;

            var result = await _validator.ValidateAsync(input, cancellationToken);
            if (result.HasFailed)
            {
                return Response.Fail<Channel>(result.Errors.Select(e => $"{e.Path} - {e.Message}"));
            }

            var channel = new Channel(Guid.NewGuid(), input.ChannelName, new[] {user.Id}.ToImmutableArray(), user.Id,
                _dateTimeProvider.UtcNow, _dateTimeProvider.UtcNow);
            await _channelRepository.InsertAsync(channel, cancellationToken);
            return Response.Ok(channel);
        }
    }
}