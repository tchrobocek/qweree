using System;
using Qweree.Validator.ModelValidation.Static;
using Xunit;

namespace Qweree.Validator.Test.ModelValidator.Static
{
    public class ModelSettingsBuilderTest
    {
        private class Model
        {
            // ReSharper disable once UnusedMember.Local
            public object Foo { get; set; } = new object();
        }

        [Fact]
        public void TestAddProperty()
        {
            var builder = new ModelSettingsBuilder(typeof(ModelSettingsBuilderTest));
            builder.AddProperty("a");

            var settings = builder.Build();
            Assert.Single(settings.PropertySettings);
        }

        [Fact]
        public void TestAddProperty_Duplicate()
        {
            var builder = new ModelSettingsBuilder(typeof(ModelSettingsBuilderTest));
            builder.AddProperty("a");
            Assert.Throws<ArgumentException>(() => builder.AddProperty("a"));
        }

        [Fact]
        public void TestAddPropertyExpr_Error()
        {
            var builder = new ModelSettingsBuilder<Model>();
            Assert.Throws<ArgumentException>(() => builder.AddProperty(m => GetType()));
        }
    }
}