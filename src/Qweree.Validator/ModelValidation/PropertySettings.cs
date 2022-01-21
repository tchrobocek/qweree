using System.Collections.Generic;
using System.Linq;

namespace Qweree.Validator.ModelValidation;

/// <summary>
///     Holds property name with property constraints.
/// </summary>
public class PropertySettings
{
    /// <summary>
    ///     Ctor.
    /// </summary>
    /// <param name="propertyName">Property name.</param>
    /// <param name="constraints">Constraints.</param>
    public PropertySettings(string propertyName, IEnumerable<IConstraint> constraints)
    {
        PropertyName = propertyName;
        Constraints = constraints.ToList()
            .AsReadOnly();
    }

    /// <summary>
    ///     Property name.
    /// </summary>
    public string PropertyName { get; }

    /// <summary>
    ///     Constraint collection.
    /// </summary>
    public IEnumerable<IConstraint> Constraints { get; }
}