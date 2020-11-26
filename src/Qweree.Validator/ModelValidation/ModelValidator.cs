using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Qweree.Validator.ModelValidation
{
    /// <summary>
    ///     Validates configured Constraint settings.
    /// </summary>
    public class ModelValidator : IObjectValidator
    {
        private readonly List<IConstraintValidator> _constraintValidators = new List<IConstraintValidator>();
        private readonly List<ModelSettings> _modelSettings = new List<ModelSettings>();

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
        public async Task ValidateAsync(ValidationContext validationContext, ValidationBuilder builder)
        {
            var modelSettings = _modelSettings.Where(s => s.SubjectType == validationContext.Subject.GetType());

            foreach (var settings in modelSettings)
            {
                await ValidateAsync(validationContext, builder, settings)
                    .ConfigureAwait(false);
            }
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
            ModelSettings settings)
        {
            foreach (var property in settings.PropertySettings)
            {
                var propInfo = validationContext.Subject.GetType().GetProperties()
                    .First(p => p.Name == property.PropertyName);

                var value = propInfo.GetValue(validationContext.Subject);

                if (value == null)
                    throw new InvalidOperationException("Value is null.");

                foreach (var constraint in property.Constraints)
                {
                    var validator = _constraintValidators.FirstOrDefault(v => v.GetType() == constraint.ValidatorType);

                    if (validator == null)
                        throw new ArgumentException($@"Missing ""{constraint.ValidatorType}"" validator.");

                    await validator.ValidateAsync(
                        new ValidationContext($"{validationContext.Path}.{propInfo.Name}", value), constraint, builder)
                        .ConfigureAwait(false);
                }
            }
        }
    }
}