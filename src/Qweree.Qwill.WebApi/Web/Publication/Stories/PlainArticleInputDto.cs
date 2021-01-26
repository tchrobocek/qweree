using System;
using System.Collections.Generic;

namespace Qweree.Qwill.WebApi.Web.Publication.Stories
{
    /// <summary>
    ///     Plain article input.
    /// </summary>
    public class PlainArticleInputDto
    {
        /// <summary>
        ///     Channel id.
        /// </summary>
        public Guid? ChannelId { get; set; }

        /// <summary>
        ///     Publication date.
        /// </summary>
        public DateTime? PublicationDate { get; set; }

        /// <summary>
        ///     Translations.
        /// </summary>
        public List<PlainArticleTranslationInputDto> Translations { get; set; } = new();
    }
}