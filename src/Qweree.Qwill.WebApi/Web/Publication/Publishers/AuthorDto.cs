using System;

namespace Qweree.Qwill.WebApi.Web.Publication.Publishers
{
    public class AuthorDto
    {
        public Guid? Id { get; set; }
        public string? Name { get; set; }
        public Guid? ChannelId { get; set; }
        public string? ChannelName { get; set; }
    }
}