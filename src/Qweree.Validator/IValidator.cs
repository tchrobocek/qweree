using System.Threading.Tasks;

namespace Qweree.Validator
{
    /// <summary>
    ///     Interface for main validator.
    /// </summary>
    public interface IValidator
    {
        /// <summary>
        ///     Validates given subject.
        /// </summary>
        /// <param name="subject">Subject.</param>
        Task<ValidationResult> ValidateAsync(object subject);

        /// <summary>
        ///     Validates given subject.
        /// </summary>
        /// <param name="path">Path to subject.</param>
        /// <param name="subject">Subject.</param>
        Task<ValidationResult> ValidateAsync(string path, object subject);
    }
}