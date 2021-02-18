namespace Qweree.Qwill.WebApi.Domain.Publishers
{
    public class ChannelCreateInput
    {
        public ChannelCreateInput(string channelName)
        {
            ChannelName = channelName;
        }

        public string ChannelName { get; }
    }
}