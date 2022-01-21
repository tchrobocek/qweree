using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Qweree.Validator.ModelValidation.Attributes;

/// <summary>
///     Attribute settings helper.
/// </summary>
public static class AttributesSettings
{
    private static PropertySettings Create(PropertyInfo propertyInfo)
    {
        var constraints = propertyInfo.GetCustomAttributes(typeof(ConstraintAttribute), true)
            .Select(a => ((ConstraintAttribute) a).CreateConstraint());

        return new PropertySettings(propertyInfo.Name, constraints);
    }

    /// <summary>
    ///     Creates settings for given type.
    /// </summary>
    /// <param name="type">Type.</param>
    /// <returns>Settings.</returns>
    public static ModelSettings Create(Type type)
    {
        var settings = type.GetProperties()
            .Select(Create)
            .Where(s => s.Constraints.Any());

        return new ModelSettings(type, settings);
    }

    /// <summary>
    ///     Creates settings from assembly attributes.
    /// </summary>
    /// <param name="assembly">Assembly.</param>
    /// <returns>Settings.</returns>
    public static IEnumerable<ModelSettings> Create(Assembly assembly)
    {
        return assembly.GetTypes()
            .Select(Create)
            .Where(e => e.PropertySettings.Any());
    }
}