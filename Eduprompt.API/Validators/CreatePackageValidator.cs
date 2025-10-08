using Eduprompt.Domain.DTOs.Package;
using FluentValidation;

namespace Eduprompt.API.Validators;

public class CreatePackageValidator : AbstractValidator<CreatePackageDto>
{
    public CreatePackageValidator()
    {
        RuleFor(x => x.CategoryID).GreaterThan(0);
        RuleFor(x => x.PackageName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Duration).GreaterThan(0);
        RuleFor(x => x.MaxUsage).GreaterThan(0);
        RuleFor(x => x.Status).NotEmpty();
    }
}

public class UpdatePackageValidator : AbstractValidator<UpdatePackageDto>
{
    public UpdatePackageValidator()
    {
        When(x => x.PackageName != null, () => RuleFor(x => x.PackageName!).NotEmpty().MaximumLength(100));
        When(x => x.Price.HasValue, () => RuleFor(x => x.Price!.Value).GreaterThanOrEqualTo(0));
        When(x => x.Duration.HasValue, () => RuleFor(x => x.Duration!.Value).GreaterThan(0));
        When(x => x.MaxUsage.HasValue, () => RuleFor(x => x.MaxUsage!.Value).GreaterThan(0));
    }
}


