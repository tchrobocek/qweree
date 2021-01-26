using System;
using System.Collections.Immutable;
using Qweree.Qwill.WebApi.Domain.Publishers;

namespace Qweree.Qwill.WebApi.Domain.Stories
{
    public class Story
    {
        public Story(Guid id, string title, string leadParagraph, string leadImage, string language, Author author,
            DateTime publishedAt, ImmutableArray<string> pages)
        {
            Id = id;
            Title = title;
            LeadParagraph = leadParagraph;
            LeadImage = leadImage;
            Language = language;
            Author = author;
            PublishedAt = publishedAt;
            Pages = pages;
        }

        public Guid Id { get; }
        public string Title { get; }
        public string LeadParagraph { get; }
        public string LeadImage { get; }
        public ImmutableArray<string> Pages { get; }
        public string Language { get; }
        public Author Author { get; }
        public DateTime PublishedAt { get; }
    }
}