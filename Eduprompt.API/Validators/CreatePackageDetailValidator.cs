using Eduprompt.Domain.DTOs.PackageDetail;
using FluentValidation;

namespace Eduprompt.API.Validators;

public class CreatePackageDetailValidator : AbstractValidator<CreatePackageDetailDto>
{
    public CreatePackageDetailValidator()
    {
        RuleFor(x => x.PackageId).GreaterThan(0);
        RuleFor(x => x.FeatureName).NotEmpty().MaximumLength(100);
        When(x => x.Unit != null, () => RuleFor(x => x.Unit!).MaximumLength(20));
        When(x => x.Limit.HasValue, () => RuleFor(x => x.Limit!.Value).GreaterThanOrEqualTo(0));
    }
}


