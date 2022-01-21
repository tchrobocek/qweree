using System;
using System.Collections.Generic;

namespace Qweree.Validator.ModelValidation.Static;

/// <summary>
///     Static configuration helper
/// </summary>
public static class StaticSettings
{
    /// <summary>
    ///     Creates settings.
    /// </summary>
    /// <param name="configureAction">Configure action.</param>
    /// <returns>Settings.</returns>
    public static IEnumerable<ModelSettings> CreateSettings(Action<ValidatorSettingsBuilder> configureAction)
    {
        var builder = new ValidatorSettingsBuilder();
        configureAction(builder);
        return builder.Build();
    }
}