using System;
using System.Collections.Immutable;
using System.Linq;
using Qweree.Mongo;
using Qweree.Qwill.WebApi.Domain.Publishers;

namespace Qweree.Qwill.WebApi.Infrastructure.Publication.Publishers
{
    public class ChannelRepository : MongoRepositoryBase<Channel, ChannelDo>
    {
        public ChannelRepository(MongoContext context) : base("channels", context)
        {
        }

        protected override Func<Channel, ChannelDo> ToDocument => channel => new ChannelDo
        {
            Id = channel.Id,
            Authors = channel.Authors.ToList(),
            ChannelName = channel.ChannelName,
            CreationDate = channel.CreationDate,
            OwnerId = channel.OwnerId,
            LastModificationDate = channel.LastModificationDate
        };

        protected override Func<ChannelDo, Channel> FromDocument => channel => new Channel(
            channel.Id ?? Guid.Empty, channel.ChannelName ?? "", channel.Authors.ToImmutableArray(),
            channel.OwnerId ?? Guid.Empty, channel.CreationDate ?? DateTime.MinValue,
            channel.LastModificationDate ?? DateTime.MinValue);
    }
}