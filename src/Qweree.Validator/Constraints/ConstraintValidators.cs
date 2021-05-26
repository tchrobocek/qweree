using System.Linq;
using Qweree.Validator.ModelValidation;

namespace Qweree.Validator.Constraints
{
    /// <summary>
    ///     Default constraints helper.
    /// </summary>
    public static class ConstraintValidators
    {
        private static readonly IConstraintValidator[] Validators =
        {
            new EmailConstraintValidator(),
            new MaxConstraintValidator(),
            new MinConstraintValidator(),
            new MaxLengthConstraintValidator(),
            new MinLengthConstraintValidator(),
            new RegexConstraintValidator(),
            new OneOfConstraintValidator(),
            new NotEmptyConstraintValidator(),
            new NotNullConstraintValidator(),
            new OneOfConstraintValidator()
        };

        /// <summary>
        ///     Collection of all default constraint validators.
        /// </summary>
        public static IConstraintValidator[] Defaults => Validators.ToArray();
    }
}