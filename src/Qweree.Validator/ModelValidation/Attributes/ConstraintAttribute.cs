using System;

namespace Qweree.Validator.ModelValidation.Attributes
{
    /// <summary>
    ///     Constraint attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public abstract class ConstraintAttribute : Attribute
    {
        /// <summary>
        ///     Creates constraint.
        /// </summary>
        /// <returns>Constraint.</returns>
        public abstract IConstraint CreateConstraint();
    }
}