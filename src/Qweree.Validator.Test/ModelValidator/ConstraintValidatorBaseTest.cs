using System;
using System.Threading.Tasks;
using Moq;
using Qweree.Validator.Constraints;
using Qweree.Validator.ModelValidation;
using Xunit;

namespace Qweree.Validator.Test.ModelValidator
{
    public class ConstraintValidatorBaseTest
    {
        [Fact]
        public async Task TestConstraintValidatorBase()
        {
            const int subject = 0;

            var constraintMock = new Mock<IConstraint>();
            var constraint = constraintMock.Object;

            var mock = new Mock<ConstraintValidatorBase<int, IConstraint>>();

            var validator = mock.Object;
            await validator.ValidateAsync(new ValidationContext("", subject, null), constraint, new ValidationBuilder());
        }

        [Fact]
        public async Task TestConstraintValidatorBase_WrongConstraint()
        {
            const int subject = 0;

            var constraintMock = new Mock<IConstraint>();
            var constraint = constraintMock.Object;

            var mock = new Mock<ConstraintValidatorBase<int, MinConstraint>>();

            var validator = mock.Object;
            await Assert.ThrowsAsync<InvalidCastException>(async () =>
                await validator.ValidateAsync(new ValidationContext("", subject, null), constraint, new ValidationBuilder()));
        }

        [Fact]
        public async Task TestConstraintValidatorBase_WrongSubject()
        {
            const string subject = "subject";

            var constraintMock = new Mock<IConstraint>();
            var constraint = constraintMock.Object;

            var mock = new Mock<ConstraintValidatorBase<int, IConstraint>>();

            var validator = mock.Object;
            await Assert.ThrowsAsync<InvalidCastException>(async () =>
                await validator.ValidateAsync(new ValidationContext("", subject, null), constraint, new ValidationBuilder()));
        }
    }
}