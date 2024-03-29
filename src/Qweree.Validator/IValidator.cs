using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Qweree.Validator;

/// <summary>
///     Interface for main validator.
/// </summary>
public interface IValidator
{
    /// <summary>
    ///     Validates given subject.
    /// </summary>
    /// <param name="subject">Subject.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<ValidationResult> ValidateAsync(object subject, CancellationToken cancellationToken = new());

    /// <summary>
    ///     Validates given subject.
    /// </summary>
    /// <param name="path">Path to subject.</param>
    /// <param name="subject">Subject.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<ValidationResult> ValidateAsync(string path, object subject, CancellationToken cancellationToken = new());

    /// <summary>
    ///     Validates given subjects.
    /// </summary>
    /// <param name="subjects">Subjects.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<ValidationResult> ValidateAsync(IEnumerable<object> subjects, CancellationToken cancellationToken = new());

    /// <summary>
    ///     Validates given subjects.
    /// </summary>
    /// <param name="subjects">Subjects.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<ValidationResult> ValidateAsync(Dictionary<string, object> subjects, CancellationToken cancellationToken = new());
}