using Qweree.Qwill.WebApi.Domain.Publishers;

namespace Qweree.Qwill.WebApi.Web.Publication.Publishers
{
    public class AuthorMapper
    {
        public static AuthorDto ToDto(Author author)
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