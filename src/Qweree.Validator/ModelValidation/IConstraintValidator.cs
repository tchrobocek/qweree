using System.Threading;
using System.Threading.Tasks;

namespace Qweree.Validator.ModelValidation;

/// <summary>
///     Validates subject by constraint.
/// </summary>
public interface IConstraintValidator
{
    /// <summary>
    ///     Validates given subject.
    /// </summary>
    /// <param name="validationContext">Validation context.</param>
    /// <param name="constraint">Constraint with validation settings.</param>
    /// <param name="builder">Validation builder</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task ValidateAsync(ValidationContext validationContext, IConstraint constraint, ValidationBuilder builder,
        CancellationToken cancellationToken = new());
}