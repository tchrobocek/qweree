using System;

namespace Qweree.Validator.ModelValidation
{
    /// <summary>
    ///     Constraint base class with generic validator type.
    /// </summary>
    /// <typeparam name="TValidatorType">Validator type, has to be inherited from IConstraintValidator.s</typeparam>
    public class Constraint<TValidatorType> : IConstraint
        where TValidatorType : IConstraintValidator
    {
        public Type ValidatorType => typeof(TValidatorType);
    }
}