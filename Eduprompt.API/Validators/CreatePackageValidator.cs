using Eduprompt.Domain.DTOs.Package;
using FluentValidation;

namespace Eduprompt.API.Validators;

public class CreatePackageValidator : AbstractValidator<CreatePackageDto>
{
    public CreatePackageValidator()
    {
        When(x => x.CategoryId.HasValue, () => RuleFor(x => x.CategoryId!.Value).GreaterThan(0));
        RuleFor(x => x.PackageName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
        When(x => x.DurationDays.HasValue, () => RuleFor(x => x.DurationDays!.Value).GreaterThan(0));
    }
}

public class UpdatePackageValidator : AbstractValidator<UpdatePackageDto>
{
    public UpdatePackageValidator()
    {
        When(x => x.PackageName != null, () => RuleFor(x => x.PackageName!).NotEmpty().MaximumLength(100));
        When(x => x.Price.HasValue, () => RuleFor(x => x.Price!.Value).GreaterThanOrEqualTo(0));
        When(x => x.DurationDays.HasValue, () => RuleFor(x => x.DurationDays!.Value).GreaterThan(0));
    }
}


