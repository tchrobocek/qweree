using System;
using System.Threading;
using System.Threading.Tasks;

namespace Qweree.Validator.ModelValidation
{
    /// <summary>
    ///     Base class for constraint validators.
    /// </summary>
    /// <typeparam name="TSubjectType">Subject type.</typeparam>
    /// <typeparam name="TConstraintType">Constraint type.</typeparam>
    public abstract class ConstraintValidatorBase<TSubjectType, TConstraintType> : IConstraintValidator
    {
        /// <summary>
        ///     Validates given subject.
        /// </summary>
        /// <param name="validationContext">Validation context.</param>
        /// <param name="constraint">Constraint.</param>
        /// <param name="builder">Validation builder.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <exception cref="InvalidCastException">Thrown when subject or constraint does not match given generic types.</exception>
        public Task ValidateAsync(ValidationContext validationContext, IConstraint constraint,
            ValidationBuilder builder, CancellationToken cancellationToken = new CancellationToken())
        {
            if (!(validationContext.Subject is TSubjectType typedValue))
                throw new InvalidCastException(@$"The value should be type of ""{typeof(TSubjectType)}"".");
            if (!(constraint is TConstraintType typedConstraint))
                throw new InvalidCastException(@$"The constraint should be type of ""{typeof(TConstraintType)}"".");

            return ValidateAsync(new ValidationContext<TSubjectType>(validationContext.Path, typedValue, validationContext.MemberInfo),
                typedConstraint, builder, cancellationToken);
        }

        /// <summary>
        ///     Abstract method head for typed validation context.
        /// </summary>
        /// <param name="validationContext"></param>
        /// <param name="constraint"></param>
        /// <param name="builder"></param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Task.</returns>
        protected abstract Task ValidateAsync(ValidationContext<TSubjectType> validationContext,
            TConstraintType constraint, ValidationBuilder builder, CancellationToken cancellationToken = new CancellationToken());
    }
}