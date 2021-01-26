using System;
using System.Collections.Immutable;
using System.Linq;
using Qweree.Qwill.WebApi.Domain.Stories;
using Qweree.Qwill.WebApi.Web.Publication.Stories;

namespace Qweree.Qwill.WebApi.Infrastructure.Publication.Stories
{
    public class PublicationInputMapper
    {
        public static PublicationInput FromDto(PlainArticleInputDto dto)
        {
            return new(dto.ChannelId ?? Guid.Empty, dto.PublicationDate ?? DateTime.MinValue,
                dto.Translations.Select(FromDto).ToImmutableArray());
        }
        public static PublicationTranslationInput FromDto(PlainArticleTranslationInputDto dto)
        {
            return new(dto.Language ?? "", dto.Title ?? "", new[] {dto.Content ?? ""}.ToImmutableArray(), dto.LeadParagraph ?? "", dto.HeaderImageSlug ?? "");
        }
    }
}