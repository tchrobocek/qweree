using System;

namespace Qweree.Qwill.WebApi.Web.Publication.Publishers
{
    public class ChannelDto
    {
        public Guid Id { get; set; }
        public string? ChannelName { get; set; }
        public DateTime CreationDate { get; set; }
    }
}