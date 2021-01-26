using System;
using System.Collections.Generic;

namespace Qweree.Qwill.WebApi.Infrastructure.Publication.Stories
{
    public class PublicationDo
    {
        public Guid? Id { get; set; }
        public Guid? ChannelId { get; set; }
        public Guid? UserId { get; set; }
        public DateTime? PublicationDate { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? LastModificationDate { get; set; }
        public List<PublicationTranslationDo> Translations { get; set; } = new();
    }

    public class PublicationTranslationDo
    {
        public string? Language { get; set; }
        public string? Title { get; set; }
        public List<string>? Pages { get; set; } = new();
        public string? LeadParagraph { get; set; }
        public string? HeaderImageSlug { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? LastModificationDate { get; set; }
    }
}