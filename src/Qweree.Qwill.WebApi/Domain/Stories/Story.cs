namespace Qweree.Qwill.WebApi.Domain.Stories
{
    public class Story
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string LeadingText { get; set; }
        public string LeadingImage { get; set; }
        public List<string> Pages { get; set; } = new();
        public string Language { get; set; }
        public Author Author { get; set; }
        public Channel Channel { get; set; }
        public DateTime PublishedAt { get; set; }
    }
}