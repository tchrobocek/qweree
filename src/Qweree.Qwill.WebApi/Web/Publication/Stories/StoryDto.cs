using System;
using System.Collections.Generic;
using Qweree.Qwill.WebApi.Web.Publication.Publisher;

namespace Qweree.Qwill.WebApi.Web.Publication.Stories
{
    public class StoryDto
    {
        public Guid? Id { get; set; }
        public string? Title { get; set; }
        public string? LeadingText { get; set; }
        public string? LeadingImage { get; set; }
        public List<string> Pages { get; set; } = new();
        public string? Language { get; set; }
        public AuthorDto? Author { get; set; }
        public DateTime? PublishedAt { get; set; }
    }
}