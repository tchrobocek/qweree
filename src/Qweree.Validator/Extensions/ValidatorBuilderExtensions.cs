using System;
using System.Reflection;
using Qweree.Validator.Constraints;
using Qweree.Validator.ModelValidation.Attributes;
using Qweree.Validator.ModelValidation.Static;

namespace Qweree.Validator.Extensions;

public static class ValidatorBuilderExtensions
{
    public static void WithAttributeModelSettings(this ValidatorBuilder @this, Type type)
    {
        @this.WithModelValidator();

        @this.WithModelSettings(new[] {AttributesSettings.Create(type)});
    }

    public static void WithAttributeModelSettings(this ValidatorBuilder @this, Assembly assembly)
    {
        @this.WithModelValidator();

        @this.WithModelSettings(AttributesSettings.Create(assembly));
    }

    public static void WithStaticModelSettings(this ValidatorBuilder @this,
        Action<ValidatorSettingsBuilder> configurationAction)
    {
        @this.WithModelValidator();

        @this.WithModelSettings(StaticSettings.CreateSettings(configurationAction));
    }

    public static void WithDefaultConstraints(this ValidatorBuilder @this)
    {
        @this.WithModelValidator();

        foreach (var constraint in ConstraintValidators.Defaults)
            @this.WithConstraintValidator(constraint);
    }
}