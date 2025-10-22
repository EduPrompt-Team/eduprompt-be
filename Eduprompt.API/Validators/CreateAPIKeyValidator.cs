using Eduprompt.Domain.DTOs.Apikey;
using FluentValidation;

namespace Eduprompt.API.Validators;

public class CreateAPIKeyValidator : AbstractValidator<CreateApikeyDto>
{
    public CreateAPIKeyValidator()
    {
        RuleFor(x => x.PackageId).GreaterThan(0);
        RuleFor(x => x.Apiprovider).NotEmpty().MaximumLength(100);
        RuleFor(x => x.KeyHash).NotEmpty().MaximumLength(500);
        When(x => x.UsageLimit.HasValue, () => RuleFor(x => x.UsageLimit!.Value).GreaterThan(0));
    }
}


