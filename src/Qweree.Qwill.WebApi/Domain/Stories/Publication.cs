using System;
using System.Collections.Immutable;

namespace Qweree.Qwill.WebApi.Domain.Stories
{
    public class Publication
    {
        public Publication(Guid publicationId, Guid authorId, DateTime publicationDate, DateTime creationDate, DateTime lastModificationDate, ImmutableArray<PublicationTranslation> translations)
        {
            PublicationId = publicationId;
            AuthorId = authorId;
            PublicationDate = publicationDate;
            CreationDate = creationDate;
            LastModificationDate = lastModificationDate;
            Translations = translations;
        }

        public Guid PublicationId { get; }
        public Guid AuthorId { get; }
        public DateTime PublicationDate { get; }
        public DateTime CreationDate { get; }
        public DateTime LastModificationDate { get; }
        public ImmutableArray<PublicationTranslation> Translations { get; }
    }

    public class PublicationTranslation
    {
        public PublicationTranslation(string language, string title, string leadParagraph, string headerImageSlug, ImmutableArray<string> pages)
        {
            Language = language;
            Title = title;
            LeadParagraph = leadParagraph;
            HeaderImageSlug = headerImageSlug;
            Pages = pages;
        }

        public string Language { get; }
        public string Title { get; }
        public ImmutableArray<string> Pages { get; }
        public string LeadParagraph { get; }
        public string HeaderImageSlug { get; }
    }
}