using System;
using System.Collections.Generic;
using System.Linq;

namespace Qweree.Validator.ModelValidation.Static;

/// <summary>
///     Validation settings builder.
/// </summary>
public class ValidatorSettingsBuilder
{
    private readonly List<ModelSettingsBuilder> _builders = new();

    /// <summary>
    ///     Adds model to settings collection.
    /// </summary>
    /// <param name="builderAction">Action to build settings.</param>
    /// <typeparam name="TModelType">Type of model class.</typeparam>
    /// <exception cref="ArgumentException">When model is already configured.</exception>
    public void AddModel<TModelType>(Action<ModelSettingsBuilder<TModelType>> builderAction)
        where TModelType : class
    {
        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
        if (_builders.Any(b => b.SubjectType == typeof(TModelType)))
            throw new ArgumentException($@"Model of type ""{typeof(TModelType)}"" was already added.");

        var builder = new ModelSettingsBuilder<TModelType>();
        _builders.Add(builder);

        builderAction(builder);
    }

    /// <summary>
    ///     Builds configuration.
    /// </summary>
    /// <returns>Built configuration.</returns>
    public IEnumerable<ModelSettings> Build()
    {
        return _builders.Select(b => b.Build());
    }
}