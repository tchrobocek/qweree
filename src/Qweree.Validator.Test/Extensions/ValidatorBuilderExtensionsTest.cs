using System;
using System.Linq;
using Moq;
using Qweree.Validator.Constraints;
using Qweree.Validator.Extensions;
using Qweree.Validator.ModelValidation.Static;
using Xunit;

namespace Qweree.Validator.Test.Extensions;

public class ValidatorBuilderExtensionsTest
{
    [Fact]
    public void TestStaticModelSettings()
    {
        var builder = new ValidatorBuilder();
        var mock = new Mock<Action<ValidatorSettingsBuilder>>();
        builder.WithStaticModelSettings(mock.Object);

        mock.Verify(m => m(It.IsAny<ValidatorSettingsBuilder>()), Times.Once);
    }

    [Fact]
    public void TestWithDefaultConstraints()
    {
        var builder = new ValidatorBuilder();
        builder.WithDefaultConstraints();
        var validator = builder.Build();

        Assert.Single(validator.ObjectValidators);
        var modelValidator = validator.ObjectValidators.Single();
        Assert.IsType<ModelValidation.ModelValidator>(modelValidator);

        var typedModelValidator = (ModelValidation.ModelValidator) modelValidator;
        Assert.Equal(ConstraintValidators.Defaults.Length, typedModelValidator.ConstraintValidators.Length);
    }

    [Fact]
    public void TestAttributesModelSettings()
    {
        var builder = new ValidatorBuilder();
        builder.WithAttributeModelSettings(typeof(TestModel));
        var validator = builder.Build();

        Assert.Single(validator.ObjectValidators);
        var modelValidator = validator.ObjectValidators.Single();
        Assert.IsType<ModelValidation.ModelValidator>(modelValidator);

        var typedModelValidator = (ModelValidation.ModelValidator) modelValidator;
        Assert.Single(typedModelValidator.ModelSettings);
    }


    [Fact]
    public void TestAttributesModelSettings_Assembly()
    {
        var builder = new ValidatorBuilder();
        builder.WithAttributeModelSettings(GetType().Assembly);
        var validator = builder.Build();

        Assert.Single(validator.ObjectValidators);
        var modelValidator = validator.ObjectValidators.Single();
        Assert.IsType<ModelValidation.ModelValidator>(modelValidator);

        var typedModelValidator = (ModelValidation.ModelValidator) modelValidator;
        Assert.NotEmpty(typedModelValidator.ModelSettings);
    }

    private class TestModel
    {
        [MinLengthConstraint(0)]
#pragma warning disable 8618
        // ReSharper disable once UnusedMember.Local
        // ReSharper disable once UnassignedGetOnlyAutoProperty
        public object Property { get; }
#pragma warning restore 8618
    }
}