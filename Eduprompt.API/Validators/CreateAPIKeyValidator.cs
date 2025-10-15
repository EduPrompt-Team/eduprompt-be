using Eduprompt.Domain.DTOs.APIKey;
using FluentValidation;

namespace Eduprompt.API.Validators;

public class CreateAPIKeyValidator : AbstractValidator<CreateAPIKeyDto>
{
    public CreateAPIKeyValidator()
    {
        RuleFor(x => x.PackageID).GreaterThan(0);
        RuleFor(x => x.APIProvider).NotEmpty().MaximumLength(100);
        RuleFor(x => x.KeyHash).NotEmpty().MaximumLength(500);
        When(x => x.UsageLimit.HasValue, () => RuleFor(x => x.UsageLimit!.Value).GreaterThan(0));
    }
}


