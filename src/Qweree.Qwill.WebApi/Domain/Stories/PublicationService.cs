using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Qweree.AspNet.Application;
using Qweree.AspNet.Session;
using Qweree.Qwill.WebApi.Domain.Publishers;
using Qweree.Utils;
using Qweree.Validator;

namespace Qweree.Qwill.WebApi.Domain.Stories
{
    public class PublicationService
    {
        private readonly IPublicationRepository _publicationRepository;
        private readonly ISessionStorage _sessionStorage;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IValidator _validator;

        public PublicationService(IPublicationRepository publicationRepository,
            ISessionStorage sessionStorage, IDateTimeProvider dateTimeProvider, IValidator validator)
        {
            _publicationRepository = publicationRepository;
            _sessionStorage = sessionStorage;
            _dateTimeProvider = dateTimeProvider;
            _validator = validator;
        }

        private PublicationTranslation CreatePublicationTranslation(PublicationTranslationInput input)
        {
            return new(input.Language, input.Title, input.LeadParagraph, input.HeaderImageSlug,
                input.Pages, _dateTimeProvider.UtcNow, _dateTimeProvider.UtcNow);
        }

        public async Task<Response<Story>> CreatePublicationAsync(PublicationInput input,
            CancellationToken cancellationToken = new())
        {
            var user = _sessionStorage.CurrentUser;

            var translations = input.Translations.Select(CreatePublicationTranslation)
                .ToArray();
            var publication = new Publication(Guid.NewGuid(), input.ChannelId, user.Id, input.PublicationDate,
                _dateTimeProvider.UtcNow, _dateTimeProvider.UtcNow, translations.ToImmutableArray());

            var validationList = new List<object>(input.Translations) {input};
            var result = await _validator.ValidateAsync(validationList, cancellationToken);

            if (result.HasFailed)
            {
                return Response.Fail<Story>(result.Errors.Select(e => $"{e.Path} - {e.Message}"));
            }

            Story story;

            try
            {
                story = ToStory(publication);
            }
            catch (Exception e)
            {
                return Response.Fail<Story>(e.Message);
            }

            try
            {
                await _publicationRepository.InsertAsync(publication, cancellationToken);
            }
            catch (Exception)
            {
                return Response.Fail<Story>("Failed to persist publication.");
            }

            return Response.Ok(story);
        }

        public async Task<Response<Story>> GetStoryAsync(Guid id, CancellationToken cancellationToken = new())
        {
            Publication publication;
            try
            {
                publication = await _publicationRepository.GetAsync(id, cancellationToken);
            }
            catch (Exception)
            {
                return Response.Fail<Story>(new Error("Story was not found.", 404));
            }

            Story story;
            try
            {
                story = ToStory(publication);
            }
            catch (Exception e)
            {
                return Response.Fail<Story>(e.Message);
            }

            return Response.Ok(story);
        }

        public Story ToStory(Publication publication)
        {
            var translation = publication.Translations.FirstOrDefault(t => t.Language == "en");

            if (translation == null)
            {
                try
                {
                    translation = publication.Translations.First();
                }
                catch (Exception)
                {
                    throw new ArgumentException("Publication does not contain any translations.");
                }
            }

            var author = new Author(publication.UserId, "user", publication.ChannelId, "channel");

            var publicationDate = publication.PublicationDate;

            if (publicationDate == DateTime.MinValue)
                publicationDate = publication.CreationDate;

            return new(publication.Id, translation.Title, translation.LeadParagraph, translation.HeaderImageSlug, translation.Language, author, publicationDate, translation.Pages);
        }
    }
}