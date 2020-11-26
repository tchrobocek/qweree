using System;
using System.Threading.Tasks;

namespace Qweree.Validator
{
    /// <summary>
    ///     Interface class for object validators.
    /// </summary>
    public interface IObjectValidator
    {
        /// <summary>
        ///     Validates context subject.
        /// </summary>
        /// <param name="validationContext">Validation context.</param>
        /// <param name="builder">Validation builder.</param>
        Task ValidateAsync(ValidationContext validationContext, ValidationBuilder builder);

        /// <summary>
        ///     Determines whether the type is validatable by derived class.
        /// </summary>
        /// <param name="type">Subject type.</param>
        /// <returns>True if it is validatable. False if it is not.</returns>
        bool Supports(Type type);
    }
}