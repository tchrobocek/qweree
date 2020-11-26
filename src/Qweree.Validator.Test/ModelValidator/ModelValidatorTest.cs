using System;
using System.Linq;
using System.Threading.Tasks;
using Qweree.Validator.ModelValidation;
using Xunit;

namespace Qweree.Validator.Test.ModelValidator
{
    public class ModelValidatorTest
    {
        private class MinConstraint : Constraint<MinConstraintValidator>
        {
            public MinConstraint(int min)
            {
                Min = min;
            }

            public int Min { get; }
        }

        private class MinConstraintValidator : ConstraintValidatorBase<int, MinConstraint>
        {
            protected override Task ValidateAsync(ValidationContext<int> context, MinConstraint constraint,
                ValidationBuilder builder)
            {
                if (context.Subject < constraint.Min)
                    builder.AddError(context.Path, "error");

                return Task.CompletedTask;
            }
        }

        private class Model
        {
            public Model(int number)
            {
                Number = number;
            }

            // ReSharper disable once UnusedMember.Local
            public int Number { get; }
        }

        [Fact]
        public async Task TestModelValidator()
        {
            var modelSettings = new[]
            {
                new ModelSettings(typeof(Model), new[]
                {
                    new PropertySettings(nameof(Model.Number), new[]
                    {
                        new MinConstraint(5)
                    })
                })
            };

            var constraintValidators = new IConstraintValidator[]
            {
                new MinConstraintValidator()
            };
            var modelValidator = new ModelValidation.ModelValidator(modelSettings, constraintValidators);

            {
                var item = new Model(1);
                Assert.True(modelValidator.Supports(item.GetType()));
                var builder = new ValidationBuilder();
                await modelValidator.ValidateAsync(new ValidationContext("", item), builder);

                var result = builder.Build();
                Assert.Single(result.Errors);
                Assert.Equal(".Number", result.Errors.First().Path);
            }
            {
                var item = new Model(5);
                Assert.True(modelValidator.Supports(item.GetType()));
                var builder = new ValidationBuilder();
                await modelValidator.ValidateAsync(new ValidationContext("", item), builder);
                Assert.Empty(builder.Build().Errors);
            }
        }

        [Fact]
        public async Task TestModelValidator_NotFound()
        {
            var modelSettings = new[]
            {
                new ModelSettings(typeof(Model), new[]
                {
                    new PropertySettings(nameof(Model.Number), new[]
                    {
                        new MinConstraint(5)
                    })
                })
            };

            var constraintValidators = new IConstraintValidator[0];
            var modelValidator = new ModelValidation.ModelValidator(modelSettings, constraintValidators);

            var item = new Model(1);
            var builder = new ValidationBuilder();
            await Assert.ThrowsAsync<ArgumentException>(async () =>
                await modelValidator.ValidateAsync(new ValidationContext("", item), builder));
        }
    }
}