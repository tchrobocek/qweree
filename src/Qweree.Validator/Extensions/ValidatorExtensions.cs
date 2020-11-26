using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Qweree.Validator.Extensions
{
    /// <summary>
    /// Validation extensions
    /// </summary>
    public static class ValidatorExtensions
    {
        /// <summary>
        /// Validates all given subjects and aggregates them into one result.
        /// </summary>
        /// <param name="this">Validator.</param>
        /// <param name="subjects">Subjects.</param>
        /// <returns>Aggregated result.</returns>
        public static async Task<ValidationResult> ValidateManyAsync(this IValidator @this, params object[] subjects)
        {
            var subjectsWithPath = subjects.Select((s, i) => new KeyValuePair<string, object>(i.ToString(), s));
            return await @this.ValidateManyAsync(subjectsWithPath);
        }

        /// <summary>
        /// Validates all given subjects and aggregates them into one result.
        /// </summary>
        /// <param name="this">Validator.</param>
        /// <param name="subjects">Subjects.</param>
        /// <returns>Aggregated result.</returns>
        public static async Task<ValidationResult> ValidateManyAsync(this IValidator @this, IEnumerable<KeyValuePair<string, object>> subjects)
        {
            var validationBuilder = new ValidationBuilder();

            // ReSharper disable once UseDeconstruction
            // not supported in netstandard2.0
            foreach (var pair in subjects)
            {
                var result = await @this.ValidateAsync(pair.Key, pair.Value);

                foreach (var error in result.Errors)
                    validationBuilder.AddError(error.Path, error.Message);

                foreach (var warning in result.Warnings)
                    validationBuilder.AddWarning(warning.Path, warning.Message);
            }

            return validationBuilder.Build();
        }
    }
}