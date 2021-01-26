using System;

namespace Qweree.Qwill.WebApi.Domain.Publishers
{
    public class Author
    {
        public Author(Guid id, string name, Guid channelId, string channelName)
        {
            Id = id;
            Name = name;
            ChannelId = channelId;
            ChannelName = channelName;
        }

        public Guid Id { get; }
        public string Name { get; }
        public Guid ChannelId { get; }
        public string ChannelName { get; }
    }
}