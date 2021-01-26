using System;
using System.Collections.Generic;

namespace Qweree.Qwill.WebApi.Infrastructure.Publication.Publishers
{
    public class ChannelDo
    {
        public Guid? Id { get; set; }
        public string? ChannelName { get; set; }
        public List<Guid> Authors { get; set; } = new List<Guid>();
        public Guid? OwnerId { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? LastModificationDate { get; set; }
    }
}