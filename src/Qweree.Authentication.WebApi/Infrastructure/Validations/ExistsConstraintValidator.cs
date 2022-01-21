using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Validator;
using Qweree.Validator.ModelValidation;

namespace Qweree.Authentication.WebApi.Infrastructure.Validations;

public class ExistsConstraintValidator : ConstraintValidatorBase<object, ExistsConstraint>
{
    private readonly IEnumerable<IExistsConstraintValidatorRepository> _repositories;

    public ExistsConstraintValidator(IEnumerable<IExistsConstraintValidatorRepository> repositories)
    {
        _repositories = repositories;
    }

    protected override async Task ValidateAsync(ValidationContext<object> validationContext,
        ExistsConstraint constraint, ValidationBuilder builder, CancellationToken cancellationToken = new())
    {
        if (validationContext.Subject == null)
            return;

        var repository = _repositories.FirstOrDefault(r => constraint.RepositoryType == r.GetType());

        if (repository == null)
            throw new ArgumentException(@$"Repository of type ""{constraint.RepositoryType}"" is not registered.");

        IEnumerable<object> subjects;

        if (validationContext.Subject is not string && validationContext.Subject is IEnumerable enumerable)
        {
            subjects = enumerable.Cast<object>();
        }
        else
        {
            subjects = new[] {validationContext.Subject};
        }

        foreach (var subject in subjects)
        {
            var isExisting = await repository.IsExistingAsync(subject.ToString() ?? string.Empty, cancellationToken);

            if (!isExisting)
                builder.AddError(validationContext.Path, $@"Entity ""{subject}"" does not exist.");
        }
    }
}

public class ExistsConstraint : Constraint<ExistsConstraintValidator>
{
    public ExistsConstraint(Type repositoryType)
    {
        RepositoryType = repositoryType;
    }

    public Type RepositoryType { get; }
}

public interface IExistsConstraintValidatorRepository
{
    Task<bool> IsExistingAsync(string value, CancellationToken cancellationToken = new());
}