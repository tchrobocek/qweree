using System;
using Qweree.Validator.ModelValidation.Static;
using Xunit;

namespace Qweree.Validator.Test.ModelValidator.Static
{
    public class ValidatorSettingsBuilderTest
    {
        [Fact]
        public void TestAddModel()
        {
            var builder = new ValidatorSettingsBuilder();
            builder.AddModel<ValidatorSettingsBuilder>(_ => { });

            var settings = builder.Build();
            Assert.Single(settings);
        }

        [Fact]
        public void TestAddModel_Duplicate()
        {
            var builder = new ValidatorSettingsBuilder();
            builder.AddModel<ValidatorSettingsBuilder>(_ => { });
            Assert.Throws<ArgumentException>(() => builder.AddModel<ValidatorSettingsBuilder>(_ => { }));
        }
    }
}