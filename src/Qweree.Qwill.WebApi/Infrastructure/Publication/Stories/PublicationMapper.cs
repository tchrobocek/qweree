using System;
using System.Collections.Immutable;
using System.Linq;
using Qweree.Qwill.WebApi.Domain.Stories;

namespace Qweree.Qwill.WebApi.Infrastructure.Publication.Stories
{
    public static class PublicationMapper
    {
        public static Domain.Stories.Publication FromDo(PublicationDo publicationDo)
        {
            return new(
                publicationDo.Id ?? Guid.Empty,
                publicationDo.ChannelId ?? Guid.Empty,
                publicationDo.UserId ?? Guid.Empty,
                publicationDo.PublicationDate ?? DateTime.MinValue,
                publicationDo.CreationDate ?? DateTime.MinValue,
                publicationDo.LastModificationDate ?? DateTime.MinValue,
                publicationDo.Translations.Select(FromDo).ToImmutableArray());
        }

        public static PublicationTranslation FromDo(PublicationTranslationDo translationDo)
        {
            return new(translationDo.Language ?? "", translationDo.Title ?? "", translationDo.LeadParagraph ?? "",
                translationDo.HeaderImageSlug ?? "", translationDo.Pages?.ToImmutableArray() ?? ImmutableArray<string>.Empty,
                translationDo.CreationDate ?? DateTime.MinValue, translationDo.LastModificationDate ?? DateTime.MinValue);
        }
        public static PublicationDo ToDo(Domain.Stories.Publication publication)
        {
            return new()
            {
                Id = publication.Id,
                Translations = publication.Translations.Select(ToDo).ToList(),
                UserId = publication.UserId,
                ChannelId = publication.ChannelId,
                CreationDate = publication.CreationDate,
                PublicationDate = publication.PublicationDate,
                LastModificationDate = publication.LastModificationDate,
            };
        }

        public static PublicationTranslationDo ToDo(PublicationTranslation translation)
        {
            return new()
            {
                Language = translation.Language,
                Pages = translation.Pages.ToList(),
                Title = translation.Title,
                CreationDate = translation.CreationDate,
                LeadParagraph = translation.LeadParagraph,
                HeaderImageSlug = translation.HeaderImageSlug,
                LastModificationDate = translation.LastModificationDate
            };
        }
    }
}