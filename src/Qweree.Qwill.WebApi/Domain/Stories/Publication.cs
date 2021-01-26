using System;
using System.Collections.Immutable;

namespace Qweree.Qwill.WebApi.Domain.Stories
{
    public class Publication
    {
        public Publication(Guid id, Guid channelId, Guid userId, DateTime publicationDate, DateTime creationDate,
            DateTime lastModificationDate, ImmutableArray<PublicationTranslation> translations)
        {
            Id = id;
            ChannelId = channelId;
            UserId = userId;
            PublicationDate = publicationDate;
            CreationDate = creationDate;
            LastModificationDate = lastModificationDate;
            Translations = translations;
        }

        public Guid Id { get; }
        public Guid ChannelId { get; }
        public Guid UserId { get; }
        public DateTime PublicationDate { get; }
        public DateTime CreationDate { get; }
        public DateTime LastModificationDate { get; }
        public ImmutableArray<PublicationTranslation> Translations { get; }
    }

    public class PublicationTranslation
    {
        public PublicationTranslation(string language, string title, string leadParagraph, string headerImageSlug,
            ImmutableArray<string> pages, DateTime creationDate, DateTime lastModificationDate)
        {
            Language = language;
            Title = title;
            LeadParagraph = leadParagraph;
            HeaderImageSlug = headerImageSlug;
            Pages = pages;
            CreationDate = creationDate;
            LastModificationDate = lastModificationDate;
        }

        public string Language { get; }
        public string Title { get; }
        public ImmutableArray<string> Pages { get; }
        public string LeadParagraph { get; }
        public string HeaderImageSlug { get; }
        public DateTime CreationDate { get; }
        public DateTime LastModificationDate { get; }
    }
}