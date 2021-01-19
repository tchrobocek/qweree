using System.Collections.Generic;

namespace Qweree.Qwill.WebApi.Web.Publication.Stories
{
    public class PlainArticleTranslationInputDto
    {
        public string? Language { get; set; }
        public string? Title { get; set; }
        public List<string> Pages { get; set; } = new();
        public string? LeadParagraph { get; set; }
        public string? HeaderImageSlug { get; set; }
    }
}