using System.Collections;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Validator.ModelValidation;
using Qweree.Validator.ModelValidation.Attributes;
using Xunit;

namespace Qweree.Validator.Test.ModelValidator.Attributes
{
    public class AttributesSettingsTest
    {
        private class MinConstraintAttribute : ConstraintAttribute
        {
            private readonly int _min;

            public MinConstraintAttribute(int min)
            {
                _min = min;
            }

            public override IConstraint CreateConstraint()
            {
                return new MinConstraint(_min);
            }
        }

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
                ValidationBuilder builder, CancellationToken cancellationToken = new CancellationToken())
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

            [MinConstraint(4)]
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public int Number { get; }
        }

        [Fact]
        public void TestCreate()
        {
            var settings = new[] {AttributesSettings.Create(typeof(Model))};

            Assert.Single((IEnumerable) settings);
            Assert.Equal(typeof(Model), settings.First().SubjectType);
            Assert.Single((IEnumerable) settings.First().PropertySettings);
            Assert.Equal("Number", settings.First().PropertySettings.First().PropertyName);
            Assert.Single((IEnumerable) settings.First().PropertySettings.First().Constraints);
            Assert.IsType<MinConstraint>(settings.First().PropertySettings.First().Constraints.First());
        }
    }
}