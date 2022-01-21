using System;

namespace Qweree.Validator.ModelValidation;

/// <summary>
///     Validation constraint.
/// </summary>
public interface IConstraint
{
    /// <summary>
    ///     Type of validator.
    /// </summary>
    Type ValidatorType { get; }
}