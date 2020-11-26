using System.Linq;
using Moq;
using Qweree.Validator.ModelValidation;
using Xunit;

namespace Qweree.Validator.Test
{
    public class ValidatorBuilderTest
    {
        [Fact]
        public void TestBuilder_Empty()
        {
            var builder = new ValidatorBuilder();
            var validator = builder.Build();

            Assert.Empty(validator.ObjectValidators);
        }

        [Fact]
        public void TestBuilder_WithCustomObjectValidator()
        {
            var builder = new ValidatorBuilder();
            var objectValidatorMock = new Mock<IObjectValidator>();
            builder.WithObjectValidator(objectValidatorMock.Object);

            var validator = builder.Build();

            Assert.Single(validator.ObjectValidators);
            Assert.Single(validator.ObjectValidators, objectValidatorMock.Object);
        }

        [Fact]
        public void TestBuilder_WithModelValidator()
        {
            var builder = new ValidatorBuilder();
            builder.WithModelValidator();
            var validator = builder.Build();

            Assert.Single(validator.ObjectValidators);
            var modelValidator = validator.ObjectValidators.Single();
            Assert.IsType<ModelValidation.ModelValidator>(modelValidator);
        }

        [Fact]
        public void TestBuilder_WithCustomConstraintValidator()
        {
            var constraintValidatorMock = new Mock<IConstraintValidator>();
            var builder = new ValidatorBuilder();
            builder.WithModelValidator();
            builder.WithConstraintValidator(constraintValidatorMock.Object);
            var validator = builder.Build();

            Assert.Single(validator.ObjectValidators);
            var modelValidator = validator.ObjectValidators.Single();
            Assert.IsType<ModelValidation.ModelValidator>(modelValidator);

            var typedModelValidator = (ModelValidation.ModelValidator) modelValidator;
            Assert.Single(typedModelValidator.ConstraintValidators);
            Assert.Single(typedModelValidator.ConstraintValidators, constraintValidatorMock.Object);
        }

        [Fact]
        public void TestBuilder_WithModelSettings()
        {
            var settings = new[] {new ModelSettings(typeof(object), new PropertySettings[0])};
            var builder = new ValidatorBuilder();
            builder.WithModelValidator();
            builder.WithModelSettings(settings);
            var validator = builder.Build();

            Assert.Single(validator.ObjectValidators);
            var modelValidator = validator.ObjectValidators.Single();
            Assert.IsType<ModelValidation.ModelValidator>(modelValidator);

            var typedModelValidator = (ModelValidation.ModelValidator) modelValidator;
            Assert.Single(typedModelValidator.ModelSettings);
            Assert.Single(typedModelValidator.ModelSettings, settings.First());
        }
    }
}