namespace Qweree.Qwill.WebApi.Web.Publication.Stories
{
    public class PlainArticleTranslationInputDto
    {
        public string? Language { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? LeadParagraph { get; set; }
        public string? HeaderImageSlug { get; set; }
    }
}