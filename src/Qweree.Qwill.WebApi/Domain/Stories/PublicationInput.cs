using System;
using System.Collections.Immutable;

namespace Qweree.Qwill.WebApi.Domain.Stories
{
    public class PublicationInput
    {
        public PublicationInput(Guid channelId, DateTime publicationDate, ImmutableArray<PublicationTranslationInput> translations)
        {
            ChannelId = channelId;
            PublicationDate = publicationDate;
            Translations = translations;
        }

        public Guid ChannelId { get; }
        public DateTime PublicationDate { get; }
        public ImmutableArray<PublicationTranslationInput> Translations { get; }

    }

    public class PublicationTranslationInput
    {
        public PublicationTranslationInput(string language, string title, ImmutableArray<string> pages, string leadParagraph, string headerImageSlug)
        {
            Language = language;
            Title = title;
            Pages = pages;
            LeadParagraph = leadParagraph;
            HeaderImageSlug = headerImageSlug;
        }

        public string Language { get; }
        public string Title { get; }
        public ImmutableArray<string> Pages { get; }
        public string LeadParagraph { get; }
        public string HeaderImageSlug { get; }
    }
}