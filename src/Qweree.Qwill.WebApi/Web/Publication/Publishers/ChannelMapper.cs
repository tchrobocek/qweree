using Qweree.Qwill.WebApi.Domain.Publishers;

namespace Qweree.Qwill.WebApi.Web.Publication.Publishers
{
    public static class ChannelMapper
    {
        public static ChannelDto ToDto(Channel channel)
        {
            return new()
            {
                Id = channel.Id,
                ChannelName = channel.ChannelName,
                CreationDate = channel.CreationDate
            };
        }
    }
}