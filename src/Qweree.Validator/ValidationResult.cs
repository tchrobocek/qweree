using System.Collections.Generic;
using System.Linq;

namespace Qweree.Validator;

/// <summary>
///     Simple validation status switch.
/// </summary>
public enum ValidationStatus
{
    /// <summary>
    ///     Indicates successful validation.
    /// </summary>
    Succeeded,

    /// <summary>
    ///     Indicates that validation has failed.
    /// </summary>
    Failed
}

/// <summary>
///     Wrapper for validation status, warnings and errors.
/// </summary>
public class ValidationResult
{
    public ValidationResult(ValidationStatus status, IEnumerable<ValidationMessage> warnings,
        IEnumerable<ValidationMessage> errors)
    {
        Status = status;
        Warnings = warnings.ToList()
            .AsReadOnly();
        Errors = errors.ToList()
            .AsReadOnly();
    }

    /// <summary>
    ///     Cached validation status.
    /// </summary>
    public ValidationStatus Status { get; }

    /// <summary>
    ///     Collection of warnings.
    /// </summary>
    public IEnumerable<ValidationMessage> Warnings { get; }

    /// <summary>
    ///     Collection of errors.
    /// </summary>
    public IEnumerable<ValidationMessage> Errors { get; }

    public bool HasSucceeded => Status == ValidationStatus.Succeeded;
    public bool HasFailed => Status == ValidationStatus.Failed;
}