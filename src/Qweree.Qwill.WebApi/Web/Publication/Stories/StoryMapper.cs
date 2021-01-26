using System.Linq;
using Qweree.Qwill.WebApi.Domain.Publishers;
using Qweree.Qwill.WebApi.Domain.Stories;
using Qweree.Qwill.WebApi.Web.Publication.Publishers;

namespace Qweree.Qwill.WebApi.Web.Publication.Stories
{
    public static class StoryMapper
    {
        public static StoryDto ToDto(Story story)
        {
            return new StoryDto
            {
                Id = story.Id,
                Author = ToDto(story.Author),
                Language = story.Language,
                Pages = story.Pages.ToList(),
                Title = story.Title,
                LeadImage = story.LeadImage,
                LeadParagraph = story.LeadParagraph,
                PublishedAt = story.PublishedAt
            };
        }

        private static AuthorDto ToDto(Author author)
        {
            return new()
            {
                Id = author.Id,
                Name = author.Name,
                ChannelId = author.ChannelId,
                ChannelName = author.ChannelName
            };
        }
    }
}