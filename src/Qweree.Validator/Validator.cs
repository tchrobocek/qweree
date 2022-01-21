using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Qweree.Validator;

/// <summary>
///     Validates given entities.
/// </summary>
public class Validator : IValidator
{
    private readonly List<IObjectValidator> _validators = new();

    /// <summary>
    ///     Ctor.
    /// </summary>
    /// <param name="objectValidators">Collection of object validators.s</param>
    public Validator(IEnumerable<IObjectValidator> objectValidators)
    {
        _validators.AddRange(objectValidators);
    }

    public IObjectValidator[] ObjectValidators => _validators.ToArray();

    /// <summary>
    ///     Validates given subjects.
    /// </summary>
    /// <param name="subject">Subject.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<ValidationResult> ValidateAsync(object subject, CancellationToken cancellationToken = new())
    {
        return await ValidateAsync("", subject, cancellationToken);
    }

    /// <summary>
    ///     Validates given subject.
    /// </summary>
    /// <param name="path">Path to subject.</param>
    /// <param name="subject">Subject.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<ValidationResult> ValidateAsync(string path, object subject,
        CancellationToken cancellationToken = new())
    {
        var builder = new ValidationBuilder();

        await ValidateModelAsync(path, subject, builder, cancellationToken)
            .ConfigureAwait(false);

        var result = builder.Build();
        return result;
    }

    /// <summary>
    ///     Validates given subjects.
    /// </summary>
    /// <param name="subjects">Subject.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<ValidationResult> ValidateAsync(IEnumerable<object> subjects,
        CancellationToken cancellationToken = new())
    {
        var builder = new ValidationBuilder();

        var i = 0;
        foreach (var subject in subjects)
        {
            i++;
            await ValidateModelAsync($"[{i}]", subject, builder, cancellationToken)
                .ConfigureAwait(false);
        }

        var result = builder.Build();
        return result;
    }

    /// <summary>
    ///     Validates given subjects.
    /// </summary>
    /// <param name="subjects">Subject.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<ValidationResult> ValidateAsync(Dictionary<string, object> subjects,
        CancellationToken cancellationToken = new())
    {
        var builder = new ValidationBuilder();

        foreach (var subject in subjects)
        {
            await ValidateModelAsync(subject.Key, subject.Value, builder, cancellationToken)
                .ConfigureAwait(false);
        }

        var result = builder.Build();
        return result;
    }

    private async Task ValidateModelAsync(string path, object subject, ValidationBuilder builder,
        CancellationToken cancellationToken = new())
    {
        var type = subject.GetType();

        var matching = _validators.Where(v => v.Supports(type));

        foreach (var validator in matching)
            await validator.ValidateAsync(new ValidationContext(path, subject, null), builder, cancellationToken)
                .ConfigureAwait(false);
    }
}