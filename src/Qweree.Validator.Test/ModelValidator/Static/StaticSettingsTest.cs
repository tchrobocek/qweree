using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Validator.ModelValidation;
using Qweree.Validator.ModelValidation.Static;
using Xunit;

namespace Qweree.Validator.Test.ModelValidator.Static
{
    public class StaticSettingsTest
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

            // ReSharper disable once UnusedMember.Local
            public int Number { get; }
        }

        [Fact]
        public void TestCreate()
        {
            var constraint = new MinConstraint(4);
            var settings = StaticSettings.CreateSettings(v =>
            {
                v.AddModel<Model>(builder =>
                {
                    builder.AddProperty(e => e.Number)
                        .AddConstraint(constraint);
                });
            }).ToArray();

            Assert.Single(settings);
            Assert.Equal(typeof(Model), settings.First().SubjectType);
            Assert.Single(settings.First().PropertySettings);
            Assert.Equal("Number", settings.First().PropertySettings.First().PropertyName);
            Assert.Single(settings.First().PropertySettings.First().Constraints);
            Assert.Same(constraint, settings.First().PropertySettings.First().Constraints.First());
        }
    }
}