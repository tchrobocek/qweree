using System.Collections.Immutable;
using Qweree.Qwill.WebApi.Domain.Publishers;
using Qweree.Qwill.WebApi.Domain.Stories;
using Qweree.Validator.Constraints;
using Qweree.Validator.ModelValidation.Static;

namespace Qweree.Qwill.WebApi.Infrastructure
{
    public static class ValidationMap
    {
        public static void ConfigureValidator(ValidatorSettingsBuilder builder)
        {
            builder.AddModel<PublicationTranslationInput>(model =>
            {
                model.AddProperty(p => p.Language)
                    .AddConstraint(new OneOfConstraint(new object[] {"en", "cs", "de", "dog-latin"}.ToImmutableArray(), "Language must be one of [en, cs, de, dog-latin]."));
                model.AddProperty(e => e.Title)
                    .AddConstraint(new MinLengthConstraint(3, "Title has to be at least 3 chars long."));
                model.AddProperty(e => e.LeadParagraph)
                    .AddConstraint(new MinLengthConstraint(3, "Lead paragraph has to be at least 3 chars long."));
            });

            builder.AddModel<ChannelCreateInput>(model =>
            {
                model.AddProperty(p => p.ChannelName)
                    .AddConstraint(new MinLengthConstraint(3, "Channel name has to be at least 3 chars long."));
            });

            builder.AddModel<CommentInput>(model =>
            {
                model.AddProperty(p => p.Text)
                    .AddConstraint(new MinLengthConstraint(1, "Comment cannot be empty."));
            });
        }
    }
}