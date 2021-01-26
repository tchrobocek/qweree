using System;
using System.Collections.Immutable;

namespace Qweree.Qwill.WebApi.Domain.Publishers
{
    public class Channel
    {
        public Channel(Guid id, string channelName, ImmutableArray<Guid> authors, Guid ownerId, DateTime creationDate,
            DateTime lastModificationDate)
        {
            Id = id;
            ChannelName = channelName;
            Authors = authors;
            OwnerId = ownerId;
            CreationDate = creationDate;
            LastModificationDate = lastModificationDate;
        }

        public Guid Id { get; }
        public string ChannelName { get; }
        public ImmutableArray<Guid> Authors { get; }
        public Guid OwnerId { get; }
        public DateTime CreationDate { get; }
        public DateTime LastModificationDate { get; }
    }
}