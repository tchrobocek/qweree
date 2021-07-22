using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Qweree.Validator.ModelValidation
{
    /// <summary>
    ///     Validates configured Constraint settings.
    /// </summary>
    public class ModelValidator : IObjectValidator
    {
        private readonly List<IConstraintValidator> _constraintValidators = new();
        private readonly List<ModelSettings> _modelSettings = new();

        /// <summary>
        ///     Ctor.
        /// </summary>
        /// <param name="modelSettings">Collection of settings.</param>
        /// <param name="constraintValidators">Collection of constraint validators.</param>
        public ModelValidator(IEnumerable<ModelSettings> modelSettings,
            IEnumerable<IConstraintValidator> constraintValidators)
        {
            _modelSettings.AddRange(modelSettings);
            _constraintValidators.AddRange(constraintValidators);
        }

        public IConstraintValidator[] ConstraintValidators => _constraintValidators.ToArray();

        public ModelSettings[] ModelSettings => _modelSettings.ToArray();


        /// <summary>
        ///     Validates given subject.
        /// </summary>
        /// <param name="validationContext">Validation context.</param>
        /// <param name="builder">Validation builder.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public async Task ValidateAsync(ValidationContext validationContext, ValidationBuilder builder,
            CancellationToken cancellationToken = new())
        {
            var modelSettings = _modelSettings.Where(s => s.SubjectType == validationContext.MemberInfo?.DeclaringType);

            foreach (var settings in modelSettings)
                await ValidateAsync(validationContext, builder, settings, cancellationToken)
                    .ConfigureAwait(false);
        }

        /// <summary>
        ///     Determines whether the type is validatable by this class.
        /// </summary>
        /// <param name="type">Subject type.</param>
        /// <returns>True if it is validatable. False if it is not.</returns>
        public bool Supports(Type type)
        {
            return true;
        }


        private async Task ValidateAsync(ValidationContext validationContext, ValidationBuilder builder,
            ModelSettings settings, CancellationToken cancellationToken = new())
        {
            foreach (var property in settings.PropertySettings)
            {
                var propInfo = validationContext.Subject?.GetType().GetProperties()
                    .FirstOrDefault(p => p.Name == property.PropertyName);

                if (propInfo == null)
                    continue;

                var value = propInfo.GetValue(validationContext.Subject);

                foreach (var constraint in property.Constraints)
                {
                    var validator = _constraintValidators.FirstOrDefault(v => v.GetType() == constraint.ValidatorType);

                    if (validator == null)
                        throw new ArgumentException($@"Missing ""{constraint.ValidatorType}"" validator.");

                    var path = validationContext.Path;
                    if (!string.IsNullOrWhiteSpace(path))
                        path += ".";
                    path += propInfo.Name;

                    var context = new ValidationContext(path, value, propInfo);
                    await validator.ValidateAsync(context,
                            constraint, builder, cancellationToken)
                        .ConfigureAwait(false);
                }
            }
        }
    }
}