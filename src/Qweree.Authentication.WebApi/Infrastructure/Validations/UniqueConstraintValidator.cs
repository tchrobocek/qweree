using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Qweree.Validator;
using Qweree.Validator.ModelValidation;

namespace Qweree.Authentication.WebApi.Infrastructure.Validations;

public class UniqueConstraintValidator : ConstraintValidatorBase<string, UniqueConstraint>
{
    private readonly IEnumerable<IUniqueConstraintValidatorRepository> _repositories;

    public UniqueConstraintValidator(IEnumerable<IUniqueConstraintValidatorRepository> repositories)
    {
        _repositories = repositories;
    }

    protected override async Task ValidateAsync(ValidationContext<string> validationContext,
        UniqueConstraint constraint, ValidationBuilder builder, CancellationToken cancellationToken = new())
    {
        if (validationContext.Subject is null)
            return;

        var repository = _repositories.FirstOrDefault(r => constraint.RepositoryType == r.GetType());

        if (repository is null)
            throw new ArgumentException(@$"Repository of type ""{constraint.RepositoryType}"" is not registered.");

        var isExisting = await repository.IsExistingAsync(validationContext.MemberInfo?.Name!,
            validationContext.Subject, cancellationToken);

        if (isExisting)
            builder.AddError(validationContext.Path, $@"Entity ""{validationContext.Subject}"" already exists.");
    }
}

public class UniqueConstraint : Constraint<UniqueConstraintValidator>
{
    public UniqueConstraint(Type repositoryType)
    {
        RepositoryType = repositoryType;
    }

    public Type RepositoryType { get; }
}

public interface IUniqueConstraintValidatorRepository
{
    Task<bool> IsExistingAsync(string field, string value, CancellationToken cancellationToken = new());
}