using System.Collections.Generic;
using System.Linq;

namespace Qweree.Validator;

/// <summary>
///     Builds validation result..
/// </summary>
public class ValidationBuilder
{
    private readonly List<ValidationMessage> _errors = new();
    private readonly List<ValidationMessage> _warnings = new();

    /// <summary>
    ///     Adds error to validation result.
    /// </summary>
    /// <param name="path">Subject path.</param>
    /// <param name="message">Validation message.</param>
    public void AddError(string path, string message)
    {
        _errors.Add(new ValidationMessage(path, message));
    }

    /// <summary>
    ///     Adds warning to validation result.
    /// </summary>
    /// <param name="path">Subject path.</param>
    /// <param name="message">Validation message.</param>
    public void AddWarning(string path, string message)
    {
        _warnings.Add(new ValidationMessage(path, message));
    }

    /// <summary>
    ///     Builds validation result.
    /// </summary>
    /// <returns>Validation result.</returns>
    public ValidationResult Build()
    {
        var status = ValidationStatus.Failed;

        if (!_errors.Any())
            status = ValidationStatus.Succeeded;

        return new ValidationResult(status, _warnings, _errors);
    }
}