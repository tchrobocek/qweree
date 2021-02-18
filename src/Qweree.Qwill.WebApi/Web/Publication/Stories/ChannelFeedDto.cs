using System;
using System.Collections.Generic;

namespace Qweree.Qwill.WebApi.Web.Publication.Stories
{
    public class ChannelFeedDto
    {
        public Guid Id { get; set; }
        public string? ChannelName { get; set; }
        public List<StoryDto> Stories { get; set; } = new();
    }
}