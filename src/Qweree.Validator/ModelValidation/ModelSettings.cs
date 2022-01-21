using System;
using System.Collections.Generic;
using System.Linq;

namespace Qweree.Validator.ModelValidation;

/// <summary>
///     Model settings with subject type and its property settings.
/// </summary>
public class ModelSettings
{
    /// <summary>
    ///     Ctor.
    /// </summary>
    /// <param name="subjectType">Subject type.</param>
    /// <param name="propertySettings">Collection of property settings.</param>
    public ModelSettings(Type subjectType, IEnumerable<PropertySettings> propertySettings)
    {
        SubjectType = subjectType;
        PropertySettings = propertySettings.ToList()
            .AsReadOnly();
    }

    /// <summary>
    ///     Subject type.
    /// </summary>
    public Type SubjectType { get; }

    /// <summary>
    ///     Collection of property settings.
    /// </summary>
    public IEnumerable<PropertySettings> PropertySettings { get; }
}