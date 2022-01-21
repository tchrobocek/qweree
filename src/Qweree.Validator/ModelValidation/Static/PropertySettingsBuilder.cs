using System.Collections.Generic;

namespace Qweree.Validator.ModelValidation.Static;

/// <summary>
///     Property settings builder
/// </summary>
public class PropertySettingsBuilder
{
    private readonly List<IConstraint> _constraints = new();

    /// <summary>
    ///     Ctor.
    /// </summary>
    /// <param name="propertyName">Property name.</param>
    public PropertySettingsBuilder(string propertyName)
    {
        PropertyName = propertyName;
    }

    /// <summary>
    ///     PropertyName.
    /// </summary>
    public string PropertyName { get; }

    /// <summary>
    ///     Adds constraint for given property.
    /// </summary>
    /// <param name="constraint">Validation constraint.</param>
    /// <returns>Fluent.</returns>
    public PropertySettingsBuilder AddConstraint(IConstraint constraint)
    {
        _constraints.Add(constraint);
        return this;
    }

    /// <summary>
    ///     Builds property settings.
    /// </summary>
    /// <returns>Built settings.</returns>
    public PropertySettings Build()
    {
        return new PropertySettings(PropertyName, _constraints);
    }
}