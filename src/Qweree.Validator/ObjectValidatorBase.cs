using System;
using System.Threading;
using System.Threading.Tasks;

namespace Qweree.Validator
{
    /// <summary>
    ///     Base class for object validators.
    /// </summary>
    /// <typeparam name="TModelType"></typeparam>
    public abstract class ObjectValidatorBase<TModelType> : IObjectValidator
    {
        /// <summary>
        ///     Validates context subject.
        /// </summary>
        /// <param name="validationContext">Validation context.</param>
        /// <param name="builder">Validation builder.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <exception cref="InvalidCastException">When subject is not the right type of model.</exception>
        public async Task ValidateAsync(ValidationContext validationContext, ValidationBuilder builder, CancellationToken cancellationToken = new CancellationToken())
        {
            if (!(validationContext.Subject is TModelType typedSubject))
                throw new InvalidCastException(
                    @$"Subject is expected to be type of ""{typeof(TModelType)}"" but got ""{validationContext.Subject.GetType()}"".");

            await ValidateAsync(new ValidationContext<TModelType>(validationContext.Path, typedSubject, validationContext.MemberInfo), builder, cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        ///     Determines whether the type is validatable by this class.
        /// </summary>
        /// <param name="type">Subject type.</param>
        /// <returns>True if it is validatable. False if it is not.</returns>
        public bool Supports(Type type)
        {
            return type == typeof(TModelType);
        }

        /// <summary>
        ///     Abstract method for derived object validators.
        /// </summary>
        /// <param name="validationContext">Validation context with typed model.</param>
        /// <param name="builder">Validation builder.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        protected abstract Task ValidateAsync(ValidationContext<TModelType> validationContext,
            ValidationBuilder builder, CancellationToken cancellationToken);
    }
}